namespace Api.Common.Options;

public class ApiOptions : BaseOptions<ApiOptions>
{
    public string AuthUser { get; set; } = null!;
    public string ConnectionString { get; set; } = null!;
}