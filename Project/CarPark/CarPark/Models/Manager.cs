namespace CarPark.Models
{
    public class Manager
    {
        public int Id { get; set; }

        public string IdentityUserId { get; set; }

        public List<Enterprise> Enterprises { get; set; }
    }
}
