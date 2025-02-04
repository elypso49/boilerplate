using Api.Common.DomainModels.Responses.Crud;
using Api.Common.Entities;
using Api.Common.Models;
using Api.Common.Repositories;

namespace Api.Common.Services;

public abstract class CrudService<TEntity, TModel>(ICrudRepository<TEntity, TModel> repository) : BaseService, ICrudService<TModel>
    where TEntity : IdentifiableEntity
    where TModel : IdentifiableModel
{
    public Task<CrudResponse<IEnumerable<TModel>>> GetAll(int? startIndex = null, int? count = null, List<string>? fieldsToExclude = null)
        => HandleDataRetrievement(async () => await repository.GetAll(startIndex, count, fieldsToExclude));

    public Task<CrudResponse<TModel>> GetById(string id)
        => HandleDataRetrievement(async () => await repository.GetById(id));

    public Task<CrudResponse<TModel>> Add(TModel element)
        => HandleDataRetrievement(async () => (await repository.Add(element)));

    public Task<CrudResponse<IEnumerable<TModel>>> AddMany(IList<TModel> elements)
        => HandleDataRetrievement(async () => (await repository.AddMany(elements)));

    public Task<CrudResponse<TModel>> Update(TModel element, string? id = null)
        => HandleDataRetrievement(async () => (await repository.Update(id ?? element.Id!, element)));

    public Task<CrudResponse<IEnumerable<TModel>>> UpdateMany(IEnumerable<TModel> element)
        => HandleDataRetrievement(async () => (await repository.UpdateMany(element)));

    public Task<CrudResponse<TModel>> Upsert(TModel element, string? id = null)
        => HandleDataRetrievement(async () => (await repository.Upsert(id ?? element.Id, element)));

    public Task<CrudResponse<TModel>> Delete(string id)
        => HandleDataRetrievement(async () => (await repository.Delete(id)));

    public Task<CrudResponse<IEnumerable<TModel>>> UpgradeData()
        => HandleDataRetrievement(async () => await repository.UpgradeData());
}