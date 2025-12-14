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
using HuurApi.Models;
using AegisViolationsAPI;
using AegisViolationsAPI.Abstr;
using AegisViolationsAPI.Helpers;

namespace NEAegisViolations.Finders
{
    public class UnlptsFinder : AHttpFinder, IAegisAPIFinder
    {
        protected static string _url = "https://unlpts.t2hosted.com";
        protected PortalHelper helper = new PortalHelper(_url, "University of Nebraska Lincoln");
        public string Name => "University of Nebraska Lincoln";
        public string State => "NE";
        public string Link => "https://unlpts.t2hosted.com";

        public event EventHandler<FinderErrorEventArgs>? Error;

        public async Task<List<ParkingViolation>> Find(string licensePlate, string state)
        {
            try
            {
                var violations = await helper.SearchCitation(licensePlate, state);

                // Set Link property for all violations
                foreach (var violation in violations)
                {
                    violation.Link = this.Link;
                }

                return violations;
            }
            catch (Exception ex)
            {
                Error?.Invoke(this, new FinderErrorEventArgs
                {
                    FinderName = Name,
                    LicensePlate = licensePlate,
                    State = state,
                    Exception = ex,
                    Message = ex.Message
                });
            }

            return new List<ParkingViolation>();
        }
    }
}
