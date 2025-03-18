namespace HCP.DTOs.DTOs.AdminManagementDTO
{
    public class UserAdminDTO
    {
        public string Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public string Status { get; set; }
        public DateTime? Birthday { get; set; }
        public string Avatar { get; set; }
    }
    public class UserAdminListDTO
    {
        public List<UserAdminDTO> Items { get; set; }
        public int totalCount { get; set; }
        public int totalPages { get; set; }
        public bool hasNext { get; set; }
        public bool hasPrevious { get; set; }
    }
}
