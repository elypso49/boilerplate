using Api.Common.Abstraction;

namespace Api.Common.Models;

public class IdentifiableModel : IIdentifiable
{
    public string? Id { get; set; }
    public string? UserId { get; set; }
    public bool? Deleted { get; set; } = false;
}