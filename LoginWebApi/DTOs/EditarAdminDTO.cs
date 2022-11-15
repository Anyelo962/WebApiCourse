using System.ComponentModel.DataAnnotations;

namespace LoginWebApi.DTOs;

public class EditarAdminDTO
{
    [Required]
    [EmailAddress]
    public string Email   { get; set; }
}