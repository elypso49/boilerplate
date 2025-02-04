using Api.Common.DomainModels.Responses.Crud;

namespace Api.Common.Services;

public abstract class BaseService
{
    protected static async Task<CrudResponse<T>> HandleDataRetrievement<T>(Func<Task<CrudResponse<T>>> process)
    {
        try
        {
            return await process();
        }
        catch (Exception e)
        {
            return ManageError<T>(e);
        }
    }

    protected static async Task<CrudResponse<IEnumerable<T>>> HandleDataRetrievement<T>(Func<Task<CrudResponse<IEnumerable<T>>>> process)
    {
        try
        {
            return await process();
        }
        catch (Exception e)
        {
            return ManageError<IEnumerable<T>>(e);
        }
    }

    private static CrudResponse<T> ManageError<T>(Exception exception, CrudResponse<T>? existingResponse = null)
    {
        Console.WriteLine($"Message : {exception.Message}{Environment.NewLine}{exception.StackTrace}");

        var errors = new List<string> { exception.Message };

        if (existingResponse != null)
        {
            existingResponse.Errors.AddRange(errors);

            return existingResponse;
        }

        return new CrudResponse<T> { Errors = errors };
    }
}