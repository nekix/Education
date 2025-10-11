using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace CarPark.ViewModels.Enterprises;

public class EnterpriseCreateEditViewModel
{
    public Guid? Id { get; set; }

    [Required(ErrorMessage = "Name is required")]
    [StringLength(255, ErrorMessage = "Name cannot be longer than 255 characters")]
    [Display(Name = "Enterprise Name")]
    public required string Name { get; set; }

    [Required(ErrorMessage = "Legal Address is required")]
    [StringLength(500, ErrorMessage = "Legal Address cannot be longer than 500 characters")]
    [Display(Name = "Legal Address")]
    public required string LegalAddress { get; set; }

    [Display(Name = "Time Zone")]
    public Guid? TimeZoneId { get; set; }

    public List<SelectListItem> AvailableTimeZones { get; set; } = new List<SelectListItem>();
}


