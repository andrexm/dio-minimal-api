using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MinimalAPI.Domain.Entities;

public class Vehicle
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; } = default!;

    [Required]
    [MaxLength(50)]
    public string Brand { get; set; } = default!;

    [Required]
    [MaxLength(50)]
    public string Model { get; set; } = default!;

    [Required]
    public int Year { get; set; } = default!;

    [Required]
    [MaxLength(30)]
    public string Name { get; set; } = default!;
}
