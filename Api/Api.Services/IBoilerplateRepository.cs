using Api.Common.Repositories;
using BoilerplateModel = Api.Common.Dtos.BoilerplateModel;

namespace Api.Services;

internal interface IBoilerplateRepository : ICrudRepository<Models.BoilerplateModel, BoilerplateModel>
{
}