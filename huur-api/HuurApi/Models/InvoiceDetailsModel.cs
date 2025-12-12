/*
 *
 * Copyright (c) 2024 Alexander Orlov.
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

using Newtonsoft.Json;
using System.Collections.Generic;

namespace HuurApi.Models
{
    /// <summary>
    /// Represents detailed invoice information including tolls and fees
    /// </summary>
    public class InvoiceDetailsModel
    {
        /// <summary>
        /// List of ExternalTollDailyInvoiceDTO
        /// </summary>
        [JsonProperty("tolls")]
        public List<ExternalTollDailyInvoice> Tolls { get; set; } = new List<ExternalTollDailyInvoice>();

        /// <summary>
        /// List of Fees
        /// </summary>
        [JsonProperty("fees")]
        public List<FeesModel> Fees { get; set; } = new List<FeesModel>();
    }
}
