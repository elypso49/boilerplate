using Api.Common.Services;
using BoilerplateModel = Api.Common.Dtos.BoilerplateModel;

namespace Api.Services;

internal class BoilerplateService(IBoilerplateRepository repository) : CrudService<Models.BoilerplateModel, BoilerplateModel>(repository), IBoilerplateService
{
}