using Api.Common.DomainModels.Responses.Crud;
using Api.Common.Entities;
using Api.Common.Models;

namespace Api.Common.Repositories;

public interface ICrudRepository<TEntity, TModel>
    where TEntity : IdentifiableEntity
    where TModel : IdentifiableModel
{
    Task<CrudResponse<IEnumerable<TModel>>> GetAll(int? startIndex = null, int? count = null, List<string>? fieldsToExclude = null);
    Task<CrudResponse<TModel>> GetById(string id);

    Task<CrudResponse<TModel>> Add(TModel model);
    Task<CrudResponse<IEnumerable<TModel>>> AddMany(IList<TModel> models);

    Task<CrudResponse<TModel>> Update(string id, TModel model);
    Task<CrudResponse<IEnumerable<TModel>>> UpdateMany(IEnumerable<TModel> models);
    Task<CrudResponse<TModel>> Upsert(string? id, TModel model);

    Task<CrudResponse<TModel>> Delete(string id);

    Task<CrudResponse<IEnumerable<TModel>>> UpgradeData();
}