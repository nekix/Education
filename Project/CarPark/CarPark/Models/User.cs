using System.ComponentModel.DataAnnotations;

namespace CarPark.Models;

public class User
{
    public int Id { get; set; }
    
    [Required]
    [StringLength(50)]
    public string Username { get; set; } = string.Empty;
    
    [Required]
    [StringLength(255)]
    public string PasswordHash { get; set; } = string.Empty;
    
    public DateTime CreationDate { get; set; } = DateTime.UtcNow;
} 