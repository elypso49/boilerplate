namespace Api.Common.Abstraction;

public interface IIdentifiable
{
    string? Id { get; set; }
    bool? Deleted { get; set; }
}
