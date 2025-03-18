namespace HCP.DTOs.DTOs.WalletDTO
{
    public class WalletTransactionWithdrawResponseDTO
    {
        public double Amount { get; set; }
        public double Current { get; set; }
        public double AfterAmount { get; set; }
        public string Type { get; set; }
        public string UserId { get; set; }
        public string FullName { get; set; }
        public string Mail { get; set; }
        public string PhoneNumber { get; set; }
        public Guid? ReferenceId { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Status { get; set; }
    }
    public class WalletTransactionDepositResponseDTO
    {
        public Guid Id { get; set; }
        public double Amount { get; set; }
        public double Current { get; set; }
        public double AfterAmount { get; set; }
        public string Type { get; set; }
        public string UserId { get; set; }
        public string FullName { get; set; }
        public string Mail { get; set; }
        public string PhoneNumber { get; set; }
        public Guid? ReferenceId { get; set; }
        public string Description { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Status { get; set; }
    }
    public class WalletWithdrawRequestDTO
    {
        public string Type { get; set; }
        public decimal Amount { get; set; }
        public string UserId { get; set; }
        public string FullName { get; set; }
        public string Mail { get; set; }
        public string PhoneNumber { get; set; }
        public Guid? ReferenceId { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Status { get; set; }
    }
    public class WalletWithdrawStaffShowDTO
    {
        public Guid Id { get; set; }
        public string Type { get; set; }
        public decimal Amount { get; set; }
        public string UserId { get; set; }
        public Guid? ReferenceId { get; set; }
        public string FullName { get; set; }
        public string Mail { get; set; }
        public string PhoneNumber { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Status { get; set; }
    }
    public class GetWalletWithdrawRequestListDTO
    {
        public List<WalletWithdrawStaffShowDTO> Items { get; set; }
        public int totalCount { get; set; }
        public int totalPages { get; set; }
        public bool hasNext { get; set; }
        public bool hasPrevious { get; set; }
    }
    public class WalletDepositRequestDTO
    {
        public Guid Id { get; set; }
        public decimal Amount { get; set; }
        public decimal Current { get; set; }
        public decimal AfterAmount { get; set; }
        public DateTime CreatedDate { get; set; }
        public string Status { get; set; }
        public string Type { get; set; }
        public string UserId { get; set; }
        public Guid? ReferenceId { get; set; }
    }
    public class RevenueHousekeeperDatasShowDTO
    {
        public double revenue { get; set; }
        public string name { get; set; }
    }
    public class RevenueHousekeeperDatasListShowDTO
    {
        public List<RevenueHousekeeperDatasShowDTO> ChartData { get; set; }
    }
    public class RefundRequestShowDTO
    {
        public Guid Id { get; set; }
        public Guid BookingId { get; set; }
        public string CustomerId { get; set; }
        public string? CustomerName { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
        public string Reason { get; set; }
        public string Status { get; set; }
        public string? AcceptBy { get; set; }
        public DateTime? ResolutionDate { get; set; }
        public decimal TotalPrice { get; set; }
    }
    public class RefundRequestShowListDTO
    {
        public List<RefundRequestShowDTO> Items { get; set; }
        public int totalCount { get; set; }
        public int totalPages { get; set; }
        public bool hasNext { get; set; }
        public bool hasPrevious { get; set; }
    }
    public class RefundRequestShowDetailDTO
    {
        public Guid Id { get; set; }
        public Guid BookingId { get; set; }
        public string CustomerId { get; set; }
        public string CustomerName { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public string Reason { get; set; }
        public string Status { get; set; }
        public string ProofOfPayment { get; set; }
        public string AcceptBy { get; set; }
        public DateTime? ResolutionDate { get; set; }
        public decimal TotalPrice { get; set; }
    }
}
