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

namespace AegisBroward.API.Models
{
    public class CaseFilingsSearchRequest
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string? CaseType { get; set; }
    }

    public class PartyNameSearchRequest
    {
        public string? LastName { get; set; }
        public string? FirstName { get; set; }
        public DateTime? Dob { get; set; }
    }

    public class NumberSearchRequest
    {
        public string? CaseNumber { get; set; }
    }

    public class PartyIdSearchRequest
    {
        public string? PartyId { get; set; }
    }

    public class HearingSearchRequest
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string? Judge { get; set; }
    }

    public class DispositionSearchRequest
    {
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string? DispositionType { get; set; }
    }
}


