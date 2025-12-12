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
    public class PaymentCard
    {
        public string? CardholderName { get; set; }
        public string? Number { get; set; }
        public string? ExpMonth { get; set; }
        public string? ExpYear { get; set; }
        public string? Cvv { get; set; }
        public string? BillingZip { get; set; }
    }

    public class CitationPaymentRequest
    {
        public string? CitationNumber { get; set; }
        public decimal Amount { get; set; }
        public PaymentCard? Card { get; set; }
    }

    public class CitationPaymentResponse
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public string? ReceiptNumber { get; set; }
        public string? AuthorizationCode { get; set; }
    }
}


