namespace MinimalAPI.Domain.ModelViews;

public record AdminMv
{
    public int Id { get; set; }
    public string Name { get; set; } = default!;
    public string Email { get; set; } = default!;
    public string Role { get; set; } = default!;
}