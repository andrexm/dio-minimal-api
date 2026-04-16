using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MinimalAPI.Domain.Entities;

public class Admin
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    [MaxLength(255)]
    public string Name { get; set; } = default!;
    
    [Required]
    [MaxLength(255)]
    public string Email { get; set; } = default!;

    [Required]
    [MaxLength(50)]
    public string Role { get; set; } = "Admin";
    
    [Required]
    [MaxLength(255)]
    public string Password { get; set; } = default!;
}