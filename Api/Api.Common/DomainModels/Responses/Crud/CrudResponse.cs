namespace Api.Common.DomainModels.Responses.Crud;

public class CrudResponse<T>
{
    public bool IsAcknowledged { get; set; }
    public long? MatchedCount { get; set; }
    public long? CreatedCount { get; set; }
    public long? ModifiedCount { get; init; }
    public string? UpsertedId { get; set; }
    public T? Data { get; set; }

    public List<string> Errors { get; set; } = [];
    public bool IsSuccess => Errors.Any() == false;

    public CrudResponse<T> ThrowException(string error)
    {
        Errors.Add(error);
        return ValidateResult();
    }

    public CrudResponse<T> ValidateResult()
    {
        if (IsAcknowledged && !Errors.Any())
            return this;

        throw new Exception(
            !Errors.Any()
                ? "An exception occured"
                : Errors.Count == 1
                    ? $"An exception occured : {Environment.NewLine}{Errors.First()}"
                    : $"Exceptions occured : {Environment.NewLine}{string.Join(Environment.NewLine, Errors)}");
    }
}