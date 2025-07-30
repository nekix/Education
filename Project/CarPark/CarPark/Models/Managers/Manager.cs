using CarPark.Models.Enterprises;

namespace CarPark.Models.Managers
{
    public class Manager
    {
        public required int Id { get; set; }

        public required string IdentityUserId { get; set; }

        public required List<Enterprise> Enterprises { get; set; }
    }
}
