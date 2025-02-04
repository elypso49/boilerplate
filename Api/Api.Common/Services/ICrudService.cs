using Api.Common.DomainModels.Responses.Crud;

namespace Api.Common.Services;

public interface ICrudService<TModel>
    where TModel : class
{
    public Task<CrudResponse<IEnumerable<TModel>>> GetAll(int? startIndex = null, int? count = null, List<string>? fieldsToExclude = null);
    public Task<CrudResponse<TModel>> GetById(string id);
    public Task<CrudResponse<TModel>> Add(TModel element);
    public Task<CrudResponse<IEnumerable<TModel>>> AddMany(IList<TModel> elements);
    public Task<CrudResponse<TModel>> Update(TModel element, string? id = null);
    public Task<CrudResponse<IEnumerable<TModel>>> UpdateMany(IEnumerable<TModel> element);
    public Task<CrudResponse<TModel>> Upsert(TModel element, string? id = null);
    public Task<CrudResponse<TModel>> Delete(string id);
    public Task<CrudResponse<IEnumerable<TModel>>> UpgradeData();
}