/*
 *
 * Copyright (c) 2025 Alexander Orlov.
 * 34 Middletown Ave Atlantic Highlands NJ 07716
 *
 * THIS SOFTWARE IS THE CONFIDENTIAL AND PROPRIETARY INFORMATION OF
 * Alexander Orlov. ("CONFIDENTIAL INFORMATION"). YOU SHALL NOT DISCLOSE
 * SUCH CONFIDENTIAL INFORMATION AND SHALL USE IT ONLY IN ACCORDANCE
 * WITH THE TERMS OF THE LICENSE AGREEMENT YOU ENTERED INTO WITH
 * Alexander Orlov.
 *
 * Author: Alexander Orlov
 *
 */

using AegisViolationsAPI;
using AegisViolationsAPI.Abstr;
using HuurApi.Models;
using System.Text.Json.Serialization;

namespace NYAegisViolations.Finders
{
    /// <summary>
    /// NYC Parking Violations finder using Socrata Open Data API
    /// Dataset: Open Parking and Camera Violations (nc67-uf89)
    /// https://data.cityofnewyork.us/City-Government/Open-Parking-and-Camera-Violations/nc67-uf89
    /// </summary>
    public class NYCSocrataFinder : ASocrataBase<NycViolationResponse>
    {
        protected override string PlateFieldName => "plate";
        protected override string StateFieldName => "state";
        protected override string OrderByFieldName => "issue_date";

        public NYCSocrataFinder() : base()
        {
            DatasetId = "nc67-uf89";
            BaseUrl = "https://data.cityofnewyork.us/resource";
            Link = $"https://data.cityofnewyork.us/City-Government/Open-Parking-and-Camera-Violations/{DatasetId}";
            Name = "NYC Socrata";
            State = "NY";
        }

        protected override ParkingViolation MapToParkingViolation(NycViolationResponse violation)
        {
            var fineAmount = ParseDecimal(violation.FineAmount);
            var penaltyAmount = ParseDecimal(violation.PenaltyAmount);
            var interestAmount = ParseDecimal(violation.InterestAmount);
            var reductionAmount = ParseDecimal(violation.ReductionAmount);
            var amountDue = ParseDecimal(violation.AmountDue);

            var paymentStatus = DeterminePaymentStatus(amountDue, violation.ViolationStatus);

            return new ParkingViolation
            {
                Id = violation.SummonsNumber,
                CitationNumber = violation.SummonsNumber,
                NoticeNumber = violation.SummonsNumber,
                Provider = 0, // NYC DOF
                Agency = $"NYC {violation.IssuingAgency ?? "DOF"}",
                Address = $"Precinct {violation.Precinct}, {violation.County} County",
                Tag = violation.Plate,
                State = violation.State,
                IssueDate = ParseDateTime(violation.IssueDate),
                Amount = amountDue > 0 ? amountDue : fineAmount + penaltyAmount + interestAmount - reductionAmount,
                Currency = "USD",
                PaymentStatus = paymentStatus,
                FineType = Const.FT_PARKING, // Parking
                Note = BuildNote(violation),
                Link = BuildSummonsLink(violation.SummonsImage),
                IsActive = amountDue > 0
            };
        }

        private static string BuildNote(NycViolationResponse violation)
        {
            var parts = new List<string>();

            if (!string.IsNullOrEmpty(violation.Violation))
                parts.Add($"Violation: {violation.Violation}");

            if (!string.IsNullOrEmpty(violation.ViolationTime))
                parts.Add($"Time: {violation.ViolationTime}");

            if (!string.IsNullOrEmpty(violation.LicenseType))
                parts.Add($"License Type: {violation.LicenseType}");

            if (!string.IsNullOrEmpty(violation.ViolationStatus))
                parts.Add($"Status: {violation.ViolationStatus}");

            var penalty = ParseDecimal(violation.PenaltyAmount);
            var interest = ParseDecimal(violation.InterestAmount);

            if (penalty > 0)
                parts.Add($"Penalty: ${penalty:F2}");

            if (interest > 0)
                parts.Add($"Interest: ${interest:F2}");

            return string.Join(" | ", parts);
        }

        private static string? BuildSummonsLink(SummonsImageInfo? summonsImage)
        {
            if (summonsImage == null || string.IsNullOrEmpty(summonsImage.Url))
                return null;

            return summonsImage.Url;
        }
    }

    /// <summary>
    /// Response model for NYC Open Parking and Camera Violations API
    /// </summary>
    public class NycViolationResponse
    {
        [JsonPropertyName("plate")]
        public string? Plate { get; set; }

        [JsonPropertyName("state")]
        public string? State { get; set; }

        [JsonPropertyName("license_type")]
        public string? LicenseType { get; set; }

        [JsonPropertyName("summons_number")]
        public string? SummonsNumber { get; set; }

        [JsonPropertyName("issue_date")]
        public string? IssueDate { get; set; }

        [JsonPropertyName("violation_time")]
        public string? ViolationTime { get; set; }

        [JsonPropertyName("violation")]
        public string? Violation { get; set; }

        [JsonPropertyName("judgment_entry_date")]
        public string? JudgmentEntryDate { get; set; }

        [JsonPropertyName("fine_amount")]
        public string? FineAmount { get; set; }

        [JsonPropertyName("penalty_amount")]
        public string? PenaltyAmount { get; set; }

        [JsonPropertyName("interest_amount")]
        public string? InterestAmount { get; set; }

        [JsonPropertyName("reduction_amount")]
        public string? ReductionAmount { get; set; }

        [JsonPropertyName("payment_amount")]
        public string? PaymentAmount { get; set; }

        [JsonPropertyName("amount_due")]
        public string? AmountDue { get; set; }

        [JsonPropertyName("precinct")]
        public string? Precinct { get; set; }

        [JsonPropertyName("county")]
        public string? County { get; set; }

        [JsonPropertyName("issuing_agency")]
        public string? IssuingAgency { get; set; }

        [JsonPropertyName("violation_status")]
        public string? ViolationStatus { get; set; }

        [JsonPropertyName("summons_image")]
        public SummonsImageInfo? SummonsImage { get; set; }
    }

    /// <summary>
    /// Summons image information
    /// </summary>
    public class SummonsImageInfo
    {
        [JsonPropertyName("url")]
        public string? Url { get; set; }

        [JsonPropertyName("description")]
        public string? Description { get; set; }
    }
}
