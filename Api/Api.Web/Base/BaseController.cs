using Microsoft.AspNetCore.Mvc;

namespace Api.Web.Base;

[Route("v{version:apiVersion}/[controller]"), ApiController]
public abstract class BaseController : ControllerBase
{
    protected async Task<IActionResult> HandleRequest(Func<Task<IActionResult>> request)
    {
        try
        {
            return await request();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error : {ex.Message}{Environment.NewLine}{LogInnerException(ex.InnerException)}");

            return BadRequest(new { message = ex.Message });
        }
    }

    private static string LogInnerException(Exception? ex, int indentLevel = 0)
    {
        var result = string.Empty;

        if (ex is null)
            return result;

        var indent = new string(' ', indentLevel * 4);

        return
            $"{Environment.NewLine}{indent}Inner Exception : {ex.Message}{Environment.NewLine}{indent}Stack Trace :{ex.StackTrace}{LogInnerException(ex.InnerException, indentLevel + 1)}";
    }
}