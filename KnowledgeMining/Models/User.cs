namespace KnowledgeMining.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Username { get; set; }
        public string Password { get; set; }
        public int RoleId { get; set; }
        public string firstName { get; set; }
        public string lastName { get; set; }
        public int GenderId { get; set; }
        public string Email { get; set; }
    }
}
