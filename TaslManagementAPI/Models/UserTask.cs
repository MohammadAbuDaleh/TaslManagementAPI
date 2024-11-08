namespace TaslManagementAPI.Models
{
    public class UserTask
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public int Status { get; set; } 
        public int Priority { get; set; } 
        public DateTime DueDate { get; set; }
        public string UserId { get; set; }
        public ApplicationUser User { get; set; }
    }
}
