using Newtonsoft.Json;

namespace HuurApi.Models;

/// <summary>
/// External Toll Daily Invoice DTO model
/// </summary>
public class ExternalTollDailyInvoice
{
    /// <summary>
    /// Unique identifier for the invoice
    /// </summary>
    [JsonProperty("id")]
    public Guid Id { get; set; }

    /// <summary>
    /// Source table name
    /// </summary>
    [JsonProperty("sourceTable")]
    public string SourceTable { get; set; } = string.Empty;

    /// <summary>
    /// Source identifier
    /// </summary>
    [JsonProperty("sourceId")]
    public Guid SourceId { get; set; }

    /// <summary>
    /// Toll ID associated with the invoice
    /// </summary>
    [JsonProperty("tollId")]
    public long TollId { get; set; }

    /// <summary>
    /// License plate number
    /// </summary>
    [JsonProperty("plateNumber")]
    public string PlateNumber { get; set; } = string.Empty;

    /// <summary>
    /// State of the license plate
    /// </summary>
    [JsonProperty("plateState")]
    public string PlateState { get; set; } = string.Empty;

    /// <summary>
    /// Plate tag
    /// </summary>
    [JsonProperty("plateTag")]
    public string? PlateTag { get; set; }

    /// <summary>
    /// Agency name
    /// </summary>
    [JsonProperty("agency")]
    public string? Agency { get; set; }

    /// <summary>
    /// Invoice amount
    /// </summary>
    [JsonProperty("amount")]
    public decimal Amount { get; set; }

    /// <summary>
    /// Amount with fees
    /// </summary>
    [JsonProperty("amountWithFee")]
    public decimal AmountWithFee { get; set; }

    /// <summary>
    /// New amount
    /// </summary>
    [JsonProperty("newAmount")]
    public decimal? NewAmount { get; set; }

    /// <summary>
    /// Payment status
    /// </summary>
    [JsonProperty("paymentStatus")]
    public int PaymentStatus { get; set; }

    /// <summary>
    /// Posting date
    /// </summary>
    [JsonProperty("postingDate")]
    public DateTime PostingDate { get; set; }

    /// <summary>
    /// Transaction date
    /// </summary>
    [JsonProperty("transactionDate")]
    public DateTime TransactionDate { get; set; }

    /// <summary>
    /// Transaction date and time
    /// </summary>
    [JsonProperty("transactionDateTime")]
    public DateTime TransactionDateTime { get; set; }

    /// <summary>
    /// Exit lane
    /// </summary>
    [JsonProperty("exitLane")]
    public string? ExitLane { get; set; }

    /// <summary>
    /// Plaza description
    /// </summary>
    [JsonProperty("plazaDescription")]
    public string? PlazaDescription { get; set; }

    /// <summary>
    /// Axle information
    /// </summary>
    [JsonProperty("axle")]
    public string? Axle { get; set; }

    /// <summary>
    /// Vehicle type code
    /// </summary>
    [JsonProperty("vehicleTypeCode")]
    public string? VehicleTypeCode { get; set; }

    /// <summary>
    /// Vehicle class
    /// </summary>
    [JsonProperty("vehicleClass")]
    public string? VehicleClass { get; set; }

    /// <summary>
    /// Description
    /// </summary>
    [JsonProperty("description")]
    public string? Description { get; set; }

    /// <summary>
    /// Prepaid status
    /// </summary>
    [JsonProperty("prepaid")]
    public string Prepaid { get; set; } = "no";

    /// <summary>
    /// Plan rate
    /// </summary>
    [JsonProperty("planRate")]
    public string? PlanRate { get; set; }

    /// <summary>
    /// Fare type
    /// </summary>
    [JsonProperty("fareType")]
    public string? FareType { get; set; }

    /// <summary>
    /// Balance text
    /// </summary>
    [JsonProperty("balanceText")]
    public string? BalanceText { get; set; }

    /// <summary>
    /// Debit text
    /// </summary>
    [JsonProperty("debitText")]
    public string? DebitText { get; set; }

    /// <summary>
    /// Credit text
    /// </summary>
    [JsonProperty("creditText")]
    public string? CreditText { get; set; }

    /// <summary>
    /// Date created
    /// </summary>
    [JsonProperty("dateCreated")]
    public DateTime DateCreated { get; set; }

    /// <summary>
    /// Date updated
    /// </summary>
    [JsonProperty("dateUpdated")]
    public DateTime DateUpdated { get; set; }

    /// <summary>
    /// Note
    /// </summary>
    [JsonProperty("note")]
    public string? Note { get; set; }

    /// <summary>
    /// Exception information
    /// </summary>
    [JsonProperty("exception")]
    public string? Exception { get; set; }

    /// <summary>
    /// Is exception flag
    /// </summary>
    [JsonProperty("isException")]
    public bool? IsException { get; set; }

    /// <summary>
    /// Completed flag
    /// </summary>
    [JsonProperty("completed")]
    public bool Completed { get; set; }

    /// <summary>
    /// Date completed
    /// </summary>
    [JsonProperty("dateCompleted")]
    public DateTime? DateCompleted { get; set; }

    /// <summary>
    /// Toll plan type
    /// </summary>
    [JsonProperty("tollPlanType")]
    public int? TollPlanType { get; set; }

    /// <summary>
    /// Toll plan description
    /// </summary>
    [JsonProperty("tollPlanDescription")]
    public string? TollPlanDescription { get; set; }

    /// <summary>
    /// Is active flag
    /// </summary>
    [JsonProperty("isActive")]
    public bool IsActive { get; set; }

    /// <summary>
    /// Entry time
    /// </summary>
    [JsonProperty("entryTime")]
    public string? EntryTime { get; set; }

    /// <summary>
    /// Exit time
    /// </summary>
    [JsonProperty("exitTime")]
    public string? ExitTime { get; set; }

    /// <summary>
    /// Entry plaza
    /// </summary>
    [JsonProperty("entryPlaza")]
    public string? EntryPlaza { get; set; }

    /// <summary>
    /// Entry lane
    /// </summary>
    [JsonProperty("entryLane")]
    public string? EntryLane { get; set; }

    /// <summary>
    /// Exit plaza
    /// </summary>
    [JsonProperty("exitPlaza")]
    public string? ExitPlaza { get; set; }

    /// <summary>
    /// Booking number
    /// </summary>
    [JsonProperty("bookingNumber")]
    public string? BookingNumber { get; set; }
}

/// <summary>
/// Request model for getting invoice by toll ID
/// </summary>
public class GetInvoiceByTollIdRequest
{
    /// <summary>
    /// Source table name
    /// </summary>
    [JsonProperty("sourceTable")]
    public string SourceTable { get; set; } = string.Empty;

    /// <summary>
    /// Source identifier
    /// </summary>
    [JsonProperty("sourceId")]
    public Guid SourceId { get; set; }

    /// <summary>
    /// Toll ID to search for
    /// </summary>
    [JsonProperty("tollId")]
    public long TollId { get; set; }

    /// <summary>
    /// License plate number
    /// </summary>
    [JsonProperty("plateNumber")]
    public string PlateNumber { get; set; } = string.Empty;

    /// <summary>
    /// State of the license plate
    /// </summary>
    [JsonProperty("plateState")]
    public string PlateState { get; set; } = string.Empty;

    /// <summary>
    /// Plate tag
    /// </summary>
    [JsonProperty("plateTag")]
    public string? PlateTag { get; set; }

    /// <summary>
    /// Agency name
    /// </summary>
    [JsonProperty("agency")]
    public string? Agency { get; set; }

    /// <summary>
    /// Invoice amount
    /// </summary>
    [JsonProperty("amount")]
    public decimal Amount { get; set; }

    /// <summary>
    /// Amount with fees
    /// </summary>
    [JsonProperty("amountWithFee")]
    public decimal AmountWithFee { get; set; }

    /// <summary>
    /// New amount
    /// </summary>
    [JsonProperty("newAmount")]
    public decimal? NewAmount { get; set; }

    /// <summary>
    /// Payment status
    /// </summary>
    [JsonProperty("paymentStatus")]
    public int PaymentStatus { get; set; }

    /// <summary>
    /// Posting date
    /// </summary>
    [JsonProperty("postingDate")]
    public DateTime PostingDate { get; set; }

    /// <summary>
    /// Transaction date
    /// </summary>
    [JsonProperty("transactionDate")]
    public DateTime TransactionDate { get; set; }

    /// <summary>
    /// Transaction date and time
    /// </summary>
    [JsonProperty("transactionDateTime")]
    public DateTime TransactionDateTime { get; set; }

    /// <summary>
    /// Exit lane
    /// </summary>
    [JsonProperty("exitLane")]
    public string? ExitLane { get; set; }

    /// <summary>
    /// Plaza description
    /// </summary>
    [JsonProperty("plazaDescription")]
    public string? PlazaDescription { get; set; }

    /// <summary>
    /// Axle information
    /// </summary>
    [JsonProperty("axle")]
    public string? Axle { get; set; }

    /// <summary>
    /// Vehicle type code
    /// </summary>
    [JsonProperty("vehicleTypeCode")]
    public string? VehicleTypeCode { get; set; }

    /// <summary>
    /// Vehicle class
    /// </summary>
    [JsonProperty("vehicleClass")]
    public string? VehicleClass { get; set; }

    /// <summary>
    /// Description
    /// </summary>
    [JsonProperty("description")]
    public string? Description { get; set; }

    /// <summary>
    /// Prepaid status
    /// </summary>
    [JsonProperty("prepaid")]
    public string Prepaid { get; set; } = "no";

    /// <summary>
    /// Plan rate
    /// </summary>
    [JsonProperty("planRate")]
    public string? PlanRate { get; set; }

    /// <summary>
    /// Fare type
    /// </summary>
    [JsonProperty("fareType")]
    public string? FareType { get; set; }

    /// <summary>
    /// Balance text
    /// </summary>
    [JsonProperty("balanceText")]
    public string? BalanceText { get; set; }

    /// <summary>
    /// Debit text
    /// </summary>
    [JsonProperty("debitText")]
    public string? DebitText { get; set; }

    /// <summary>
    /// Credit text
    /// </summary>
    [JsonProperty("creditText")]
    public string? CreditText { get; set; }

    /// <summary>
    /// Note
    /// </summary>
    [JsonProperty("note")]
    public string? Note { get; set; }

    /// <summary>
    /// Exception information
    /// </summary>
    [JsonProperty("exception")]
    public string? Exception { get; set; }

    /// <summary>
    /// Is exception flag
    /// </summary>
    [JsonProperty("isException")]
    public bool? IsException { get; set; }

    /// <summary>
    /// Completed flag
    /// </summary>
    [JsonProperty("completed")]
    public bool Completed { get; set; }

    /// <summary>
    /// Date completed
    /// </summary>
    [JsonProperty("dateCompleted")]
    public DateTime? DateCompleted { get; set; }

    /// <summary>
    /// Toll plan type
    /// </summary>
    [JsonProperty("tollPlanType")]
    public int? TollPlanType { get; set; }

    /// <summary>
    /// Toll plan description
    /// </summary>
    [JsonProperty("tollPlanDescription")]
    public string? TollPlanDescription { get; set; }

    /// <summary>
    /// Is active flag
    /// </summary>
    [JsonProperty("isActive")]
    public bool IsActive { get; set; }
}

/// <summary>
/// Request model for getting invoice by plate number
/// </summary>
public class GetInvoiceByPlateRequest
{
    /// <summary>
    /// Source table name
    /// </summary>
    [JsonProperty("sourceTable")]
    public string SourceTable { get; set; } = string.Empty;

    /// <summary>
    /// Source identifier
    /// </summary>
    [JsonProperty("sourceId")]
    public Guid SourceId { get; set; }

    /// <summary>
    /// Toll ID associated with the invoice
    /// </summary>
    [JsonProperty("tollId")]
    public long TollId { get; set; }

    /// <summary>
    /// License plate number to search for
    /// </summary>
    [JsonProperty("plateNumber")]
    public string PlateNumber { get; set; } = string.Empty;

    /// <summary>
    /// State of the license plate
    /// </summary>
    [JsonProperty("plateState")]
    public string PlateState { get; set; } = string.Empty;

    /// <summary>
    /// Plate tag
    /// </summary>
    [JsonProperty("plateTag")]
    public string? PlateTag { get; set; }

    /// <summary>
    /// Agency name
    /// </summary>
    [JsonProperty("agency")]
    public string? Agency { get; set; }

    /// <summary>
    /// Invoice amount
    /// </summary>
    [JsonProperty("amount")]
    public decimal Amount { get; set; }

    /// <summary>
    /// Amount with fees
    /// </summary>
    [JsonProperty("amountWithFee")]
    public decimal AmountWithFee { get; set; }

    /// <summary>
    /// New amount
    /// </summary>
    [JsonProperty("newAmount")]
    public decimal? NewAmount { get; set; }

    /// <summary>
    /// Payment status
    /// </summary>
    [JsonProperty("paymentStatus")]
    public int PaymentStatus { get; set; }

    /// <summary>
    /// Posting date
    /// </summary>
    [JsonProperty("postingDate")]
    public DateTime PostingDate { get; set; }

    /// <summary>
    /// Transaction date
    /// </summary>
    [JsonProperty("transactionDate")]
    public DateTime TransactionDate { get; set; }

    /// <summary>
    /// Transaction date and time
    /// </summary>
    [JsonProperty("transactionDateTime")]
    public DateTime TransactionDateTime { get; set; }

    /// <summary>
    /// Exit lane
    /// </summary>
    [JsonProperty("exitLane")]
    public string? ExitLane { get; set; }

    /// <summary>
    /// Plaza description
    /// </summary>
    [JsonProperty("plazaDescription")]
    public string? PlazaDescription { get; set; }

    /// <summary>
    /// Axle information
    /// </summary>
    [JsonProperty("axle")]
    public string? Axle { get; set; }

    /// <summary>
    /// Vehicle type code
    /// </summary>
    [JsonProperty("vehicleTypeCode")]
    public string? VehicleTypeCode { get; set; }

    /// <summary>
    /// Vehicle class
    /// </summary>
    [JsonProperty("vehicleClass")]
    public string? VehicleClass { get; set; }

    /// <summary>
    /// Description
    /// </summary>
    [JsonProperty("description")]
    public string? Description { get; set; }

    /// <summary>
    /// Prepaid status
    /// </summary>
    [JsonProperty("prepaid")]
    public string Prepaid { get; set; } = "no";

    /// <summary>
    /// Plan rate
    /// </summary>
    [JsonProperty("planRate")]
    public string? PlanRate { get; set; }

    /// <summary>
    /// Fare type
    /// </summary>
    [JsonProperty("fareType")]
    public string? FareType { get; set; }

    /// <summary>
    /// Balance text
    /// </summary>
    [JsonProperty("balanceText")]
    public string? BalanceText { get; set; }

    /// <summary>
    /// Debit text
    /// </summary>
    [JsonProperty("debitText")]
    public string? DebitText { get; set; }

    /// <summary>
    /// Credit text
    /// </summary>
    [JsonProperty("creditText")]
    public string? CreditText { get; set; }

    /// <summary>
    /// Note
    /// </summary>
    [JsonProperty("note")]
    public string? Note { get; set; }

    /// <summary>
    /// Exception information
    /// </summary>
    [JsonProperty("exception")]
    public string? Exception { get; set; }

    /// <summary>
    /// Is exception flag
    /// </summary>
    [JsonProperty("isException")]
    public bool? IsException { get; set; }

    /// <summary>
    /// Completed flag
    /// </summary>
    [JsonProperty("completed")]
    public bool Completed { get; set; }

    /// <summary>
    /// Date completed
    /// </summary>
    [JsonProperty("dateCompleted")]
    public DateTime? DateCompleted { get; set; }

    /// <summary>
    /// Toll plan type
    /// </summary>
    [JsonProperty("tollPlanType")]
    public int? TollPlanType { get; set; }

    /// <summary>
    /// Toll plan description
    /// </summary>
    [JsonProperty("tollPlanDescription")]
    public string? TollPlanDescription { get; set; }

    /// <summary>
    /// Is active flag
    /// </summary>
    [JsonProperty("isActive")]
    public bool IsActive { get; set; }
}

/// <summary>
/// Request model for creating a new invoice
/// </summary>
public class CreateExternalTollDailyInvoiceRequest
{
    /// <summary>
    /// Source table name
    /// </summary>
    [JsonProperty("sourceTable")]
    public string SourceTable { get; set; } = string.Empty;

    /// <summary>
    /// Source identifier
    /// </summary>
    [JsonProperty("sourceId")]
    public Guid SourceId { get; set; }

    /// <summary>
    /// Toll ID associated with the invoice
    /// </summary>
    [JsonProperty("tollId")]
    public long TollId { get; set; }

    /// <summary>
    /// License plate number
    /// </summary>
    [JsonProperty("plateNumber")]
    public string PlateNumber { get; set; } = string.Empty;

    /// <summary>
    /// State of the license plate
    /// </summary>
    [JsonProperty("plateState")]
    public string PlateState { get; set; } = string.Empty;

    /// <summary>
    /// Plate tag
    /// </summary>
    [JsonProperty("plateTag")]
    public string? PlateTag { get; set; }

    /// <summary>
    /// Agency name
    /// </summary>
    [JsonProperty("agency")]
    public string? Agency { get; set; }

    /// <summary>
    /// Invoice amount
    /// </summary>
    [JsonProperty("amount")]
    public decimal Amount { get; set; }

    /// <summary>
    /// Amount with fees
    /// </summary>
    [JsonProperty("amountWithFee")]
    public decimal AmountWithFee { get; set; }

    /// <summary>
    /// New amount
    /// </summary>
    [JsonProperty("newAmount")]
    public decimal? NewAmount { get; set; }

    /// <summary>
    /// Payment status
    /// </summary>
    [JsonProperty("paymentStatus")]
    public int PaymentStatus { get; set; }

    /// <summary>
    /// Posting date
    /// </summary>
    [JsonProperty("postingDate")]
    public DateTime PostingDate { get; set; }

    /// <summary>
    /// Transaction date
    /// </summary>
    [JsonProperty("transactionDate")]
    public DateTime TransactionDate { get; set; }

    /// <summary>
    /// Transaction date and time
    /// </summary>
    [JsonProperty("transactionDateTime")]
    public DateTime TransactionDateTime { get; set; }

    /// <summary>
    /// Exit lane
    /// </summary>
    [JsonProperty("exitLane")]
    public string? ExitLane { get; set; }

    /// <summary>
    /// Plaza description
    /// </summary>
    [JsonProperty("plazaDescription")]
    public string? PlazaDescription { get; set; }

    /// <summary>
    /// Axle information
    /// </summary>
    [JsonProperty("axle")]
    public string? Axle { get; set; }

    /// <summary>
    /// Vehicle type code
    /// </summary>
    [JsonProperty("vehicleTypeCode")]
    public string? VehicleTypeCode { get; set; }

    /// <summary>
    /// Vehicle class
    /// </summary>
    [JsonProperty("vehicleClass")]
    public string? VehicleClass { get; set; }

    /// <summary>
    /// Description
    /// </summary>
    [JsonProperty("description")]
    public string? Description { get; set; }

    /// <summary>
    /// Prepaid status
    /// </summary>
    [JsonProperty("prepaid")]
    public string Prepaid { get; set; } = "no";

    /// <summary>
    /// Plan rate
    /// </summary>
    [JsonProperty("planRate")]
    public string? PlanRate { get; set; }

    /// <summary>
    /// Fare type
    /// </summary>
    [JsonProperty("fareType")]
    public string? FareType { get; set; }

    /// <summary>
    /// Balance text
    /// </summary>
    [JsonProperty("balanceText")]
    public string? BalanceText { get; set; }

    /// <summary>
    /// Debit text
    /// </summary>
    [JsonProperty("debitText")]
    public string? DebitText { get; set; }

    /// <summary>
    /// Credit text
    /// </summary>
    [JsonProperty("creditText")]
    public string? CreditText { get; set; }

    /// <summary>
    /// Note
    /// </summary>
    [JsonProperty("note")]
    public string? Note { get; set; }

    /// <summary>
    /// Exception information
    /// </summary>
    [JsonProperty("exception")]
    public string? Exception { get; set; }

    /// <summary>
    /// Is exception flag
    /// </summary>
    [JsonProperty("isException")]
    public bool? IsException { get; set; }

    /// <summary>
    /// Completed flag
    /// </summary>
    [JsonProperty("completed")]
    public bool Completed { get; set; }

    /// <summary>
    /// Date completed
    /// </summary>
    [JsonProperty("dateCompleted")]
    public DateTime? DateCompleted { get; set; }

    /// <summary>
    /// Toll plan type
    /// </summary>
    [JsonProperty("tollPlanType")]
    public int? TollPlanType { get; set; }

    /// <summary>
    /// Toll plan description
    /// </summary>
    [JsonProperty("tollPlanDescription")]
    public string? TollPlanDescription { get; set; }

    /// <summary>
    /// Is active flag
    /// </summary>
    [JsonProperty("isActive")]
    public bool IsActive { get; set; }
}

/// <summary>
/// Request model for updating an existing invoice
/// </summary>
public class UpdateExternalTollDailyInvoiceRequest
{
    /// <summary>
    /// Source table name
    /// </summary>
    [JsonProperty("sourceTable")]
    public string SourceTable { get; set; } = string.Empty;

    /// <summary>
    /// Source identifier
    /// </summary>
    [JsonProperty("sourceId")]
    public Guid SourceId { get; set; }

    /// <summary>
    /// Toll ID associated with the invoice
    /// </summary>
    [JsonProperty("tollId")]
    public long TollId { get; set; }

    /// <summary>
    /// License plate number
    /// </summary>
    [JsonProperty("plateNumber")]
    public string PlateNumber { get; set; } = string.Empty;

    /// <summary>
    /// State of the license plate
    /// </summary>
    [JsonProperty("plateState")]
    public string PlateState { get; set; } = string.Empty;

    /// <summary>
    /// Plate tag
    /// </summary>
    [JsonProperty("plateTag")]
    public string? PlateTag { get; set; }

    /// <summary>
    /// Agency name
    /// </summary>
    [JsonProperty("agency")]
    public string? Agency { get; set; }

    /// <summary>
    /// Invoice amount
    /// </summary>
    [JsonProperty("amount")]
    public decimal Amount { get; set; }

    /// <summary>
    /// Amount with fees
    /// </summary>
    [JsonProperty("amountWithFee")]
    public decimal AmountWithFee { get; set; }

    /// <summary>
    /// New amount
    /// </summary>
    [JsonProperty("newAmount")]
    public decimal? NewAmount { get; set; }

    /// <summary>
    /// Payment status
    /// </summary>
    [JsonProperty("paymentStatus")]
    public int PaymentStatus { get; set; }

    /// <summary>
    /// Posting date
    /// </summary>
    [JsonProperty("postingDate")]
    public DateTime PostingDate { get; set; }

    /// <summary>
    /// Transaction date
    /// </summary>
    [JsonProperty("transactionDate")]
    public DateTime TransactionDate { get; set; }

    /// <summary>
    /// Transaction date and time
    /// </summary>
    [JsonProperty("transactionDateTime")]
    public DateTime TransactionDateTime { get; set; }

    /// <summary>
    /// Exit lane
    /// </summary>
    [JsonProperty("exitLane")]
    public string? ExitLane { get; set; }

    /// <summary>
    /// Plaza description
    /// </summary>
    [JsonProperty("plazaDescription")]
    public string? PlazaDescription { get; set; }

    /// <summary>
    /// Axle information
    /// </summary>
    [JsonProperty("axle")]
    public string? Axle { get; set; }

    /// <summary>
    /// Vehicle type code
    /// </summary>
    [JsonProperty("vehicleTypeCode")]
    public string? VehicleTypeCode { get; set; }

    /// <summary>
    /// Vehicle class
    /// </summary>
    [JsonProperty("vehicleClass")]
    public string? VehicleClass { get; set; }

    /// <summary>
    /// Description
    /// </summary>
    [JsonProperty("description")]
    public string? Description { get; set; }

    /// <summary>
    /// Prepaid status
    /// </summary>
    [JsonProperty("prepaid")]
    public string Prepaid { get; set; } = "no";

    /// <summary>
    /// Plan rate
    /// </summary>
    [JsonProperty("planRate")]
    public string? PlanRate { get; set; }

    /// <summary>
    /// Fare type
    /// </summary>
    [JsonProperty("fareType")]
    public string? FareType { get; set; }

    /// <summary>
    /// Balance text
    /// </summary>
    [JsonProperty("balanceText")]
    public string? BalanceText { get; set; }

    /// <summary>
    /// Debit text
    /// </summary>
    [JsonProperty("debitText")]
    public string? DebitText { get; set; }

    /// <summary>
    /// Credit text
    /// </summary>
    [JsonProperty("creditText")]
    public string? CreditText { get; set; }

    /// <summary>
    /// Note
    /// </summary>
    [JsonProperty("note")]
    public string? Note { get; set; }

    /// <summary>
    /// Exception information
    /// </summary>
    [JsonProperty("exception")]
    public string? Exception { get; set; }

    /// <summary>
    /// Is exception flag
    /// </summary>
    [JsonProperty("isException")]
    public bool? IsException { get; set; }

    /// <summary>
    /// Completed flag
    /// </summary>
    [JsonProperty("completed")]
    public bool Completed { get; set; }

    /// <summary>
    /// Date completed
    /// </summary>
    [JsonProperty("dateCompleted")]
    public DateTime? DateCompleted { get; set; }

    /// <summary>
    /// Toll plan type
    /// </summary>
    [JsonProperty("tollPlanType")]
    public int? TollPlanType { get; set; }

    /// <summary>
    /// Toll plan description
    /// </summary>
    [JsonProperty("tollPlanDescription")]
    public string? TollPlanDescription { get; set; }

    /// <summary>
    /// Is active flag
    /// </summary>
    [JsonProperty("isActive")]
    public bool IsActive { get; set; }
}

/// <summary>
/// Response model for getting all invoices
/// </summary>
public class GetAllExternalTollDailyInvoicesResponse
{
    /// <summary>
    /// Response reason code
    /// </summary>
    [JsonProperty("reason")]
    public int Reason { get; set; }

    /// <summary>
    /// Response message
    /// </summary>
    [JsonProperty("message")]
    public string? Message { get; set; }

    /// <summary>
    /// Stack trace for errors
    /// </summary>
    [JsonProperty("stackTrace")]
    public string? StackTrace { get; set; }

    /// <summary>
    /// List of invoices
    /// </summary>
    [JsonProperty("result")]
    public List<ExternalTollDailyInvoice> Result { get; set; } = new List<ExternalTollDailyInvoice>();
}

/// <summary>
/// Response model for getting invoice by ID
/// </summary>
public class GetExternalTollDailyInvoiceByIdResponse
{
    /// <summary>
    /// Response reason code
    /// </summary>
    [JsonProperty("reason")]
    public int Reason { get; set; }

    /// <summary>
    /// Response message
    /// </summary>
    [JsonProperty("message")]
    public string? Message { get; set; }

    /// <summary>
    /// Stack trace for errors
    /// </summary>
    [JsonProperty("stackTrace")]
    public string? StackTrace { get; set; }

    /// <summary>
    /// Invoice
    /// </summary>
    [JsonProperty("result")]
    public ExternalTollDailyInvoice? Result { get; set; }
}

/// <summary>
/// Response model for getting invoice by toll ID
/// </summary>
public class GetExternalTollDailyInvoiceByTollIdResponse
{
    /// <summary>
    /// Response reason code
    /// </summary>
    [JsonProperty("reason")]
    public int Reason { get; set; }

    /// <summary>
    /// Response message
    /// </summary>
    [JsonProperty("message")]
    public string? Message { get; set; }

    /// <summary>
    /// Stack trace for errors
    /// </summary>
    [JsonProperty("stackTrace")]
    public string? StackTrace { get; set; }

    /// <summary>
    /// Matching invoices
    /// </summary>
    [JsonProperty("result")]
    public List<ExternalTollDailyInvoice> Result { get; set; } = new List<ExternalTollDailyInvoice>();
}

/// <summary>
/// Response model for getting invoice by plate
/// </summary>
public class GetExternalTollDailyInvoiceByPlateResponse
{
    /// <summary>
    /// Response reason code
    /// </summary>
    [JsonProperty("reason")]
    public int Reason { get; set; }

    /// <summary>
    /// Response message
    /// </summary>
    [JsonProperty("message")]
    public string? Message { get; set; }

    /// <summary>
    /// Stack trace for errors
    /// </summary>
    [JsonProperty("stackTrace")]
    public string? StackTrace { get; set; }

    /// <summary>
    /// Matching invoices
    /// </summary>
    [JsonProperty("result")]
    public List<ExternalTollDailyInvoice> Result { get; set; } = new List<ExternalTollDailyInvoice>();
}

/// <summary>
/// Response model for creating an invoice
/// </summary>
public class CreateExternalTollDailyInvoiceResponse
{
    /// <summary>
    /// Response reason code
    /// </summary>
    [JsonProperty("reason")]
    public int Reason { get; set; }

    /// <summary>
    /// Response message
    /// </summary>
    [JsonProperty("message")]
    public string? Message { get; set; }

    /// <summary>
    /// Stack trace for errors
    /// </summary>
    [JsonProperty("stackTrace")]
    public string? StackTrace { get; set; }

    /// <summary>
    /// Created invoice
    /// </summary>
    [JsonProperty("result")]
    public ExternalTollDailyInvoice? Result { get; set; }
}

/// <summary>
/// Response model for updating an invoice
/// </summary>
public class UpdateExternalTollDailyInvoiceResponse
{
    /// <summary>
    /// Response reason code
    /// </summary>
    [JsonProperty("reason")]
    public int Reason { get; set; }

    /// <summary>
    /// Response message
    /// </summary>
    [JsonProperty("message")]
    public string? Message { get; set; }

    /// <summary>
    /// Stack trace for errors
    /// </summary>
    [JsonProperty("stackTrace")]
    public string? StackTrace { get; set; }

    /// <summary>
    /// Updated invoice
    /// </summary>
    [JsonProperty("result")]
    public ExternalTollDailyInvoice? Result { get; set; }
}

/// <summary>
/// Response model for deleting an invoice
/// </summary>
public class DeleteExternalTollDailyInvoiceResponse
{
    /// <summary>
    /// Response reason code
    /// </summary>
    [JsonProperty("reason")]
    public int Reason { get; set; }

    /// <summary>
    /// Response message
    /// </summary>
    [JsonProperty("message")]
    public string? Message { get; set; }

    /// <summary>
    /// Stack trace for errors
    /// </summary>
    [JsonProperty("stackTrace")]
    public string? StackTrace { get; set; }

    /// <summary>
    /// Deletion result
    /// </summary>
    [JsonProperty("result")]
    public bool Result { get; set; }
}

/// <summary>
/// Request model for updating payment status of multiple invoices
/// </summary>
public class UpdatePaymentStatusRequest
{
    /// <summary>
    /// List of invoice IDs to update
    /// </summary>
    [JsonProperty("ids")]
    public List<Guid> Ids { get; set; } = new List<Guid>();

    /// <summary>
    /// New payment status to set
    /// </summary>
    [JsonProperty("paymentStatus")]
    public int PaymentStatus { get; set; }

    /// <summary>
    /// Company ID associated with the update
    /// </summary>
    [JsonProperty("companyId")]
    public string CompanyId { get; set; } = string.Empty;
}

/// <summary>
/// Response model for updating payment status
/// </summary>
public class UpdatePaymentStatusResponse
{
    /// <summary>
    /// Response reason code
    /// </summary>
    [JsonProperty("reason")]
    public int Reason { get; set; }

    /// <summary>
    /// Response message
    /// </summary>
    [JsonProperty("message")]
    public string? Message { get; set; }

    /// <summary>
    /// Stack trace for errors
    /// </summary>
    [JsonProperty("stackTrace")]
    public string? StackTrace { get; set; }

    /// <summary>
    /// Update result
    /// </summary>
    [JsonProperty("result")]
    public bool Result { get; set; }
}

/// <summary>
/// Payment information model for company invoices
/// </summary>
public class CompanyInvoicePayment
{
    /// <summary>
    /// Payment ID
    /// </summary>
    [JsonProperty("id")]
    public Guid Id { get; set; }

    /// <summary>
    /// Table name
    /// </summary>
    [JsonProperty("table")]
    public string Table { get; set; } = string.Empty;

    /// <summary>
    /// Payment name
    /// </summary>
    [JsonProperty("name")]
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Payment amount
    /// </summary>
    [JsonProperty("amount")]
    public decimal Amount { get; set; }

    /// <summary>
    /// Original payment status
    /// </summary>
    [JsonProperty("originalPaymentStatus")]
    public int OriginalPaymentStatus { get; set; }

    /// <summary>
    /// Original number
    /// </summary>
    [JsonProperty("originalNumber")]
    public string OriginalNumber { get; set; } = string.Empty;
}

/// <summary>
/// Company invoice model
/// </summary>
public class CompanyInvoice
{
    /// <summary>
    /// Invoice ID
    /// </summary>
    [JsonProperty("id")]
    public Guid Id { get; set; }

    /// <summary>
    /// Company ID
    /// </summary>
    [JsonProperty("companyId")]
    public Guid CompanyId { get; set; }

    /// <summary>
    /// Invoice date
    /// </summary>
    [JsonProperty("invoiceDate")]
    public DateTime InvoiceDate { get; set; }

    /// <summary>
    /// Payment status
    /// </summary>
    [JsonProperty("paymentStatus")]
    public int PaymentStatus { get; set; }

    /// <summary>
    /// Total amount
    /// </summary>
    [JsonProperty("totalAmount")]
    public decimal TotalAmount { get; set; }

    /// <summary>
    /// List of payments
    /// </summary>
    [JsonProperty("payments")]
    public List<CompanyInvoicePayment> Payments { get; set; } = new List<CompanyInvoicePayment>();
}

/// <summary>
/// Response model for getting invoices by company
/// </summary>
public class GetExternalTollDailyInvoiceByCompanyResponse
{
    /// <summary>
    /// Response reason code
    /// </summary>
    [JsonProperty("reason")]
    public int Reason { get; set; }

    /// <summary>
    /// Response message
    /// </summary>
    [JsonProperty("message")]
    public string? Message { get; set; }

    /// <summary>
    /// Stack trace for errors
    /// </summary>
    [JsonProperty("stackTrace")]
    public string? StackTrace { get; set; }

    /// <summary>
    /// Company invoice
    /// </summary>
    [JsonProperty("result")]
    public CompanyInvoice? Result { get; set; }
}
