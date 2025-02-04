using Api.Common.Consts;
using Api.Common.Options;
using Api.Common.Repositories;
using AutoMapper;
using Microsoft.Extensions.Options;
using BoilerplateModel = Api.Common.Dtos.BoilerplateModel;

namespace Api.Services.Repositories;

public class BoilerplateRepository(IOptions<ApiOptions> settings, IMapper mapper)
    : CrudRepository<Models.BoilerplateModel, BoilerplateModel>(settings.Value.ConnectionString, EnvironmentVariables.DatabaseName, EnvironmentVariables.BoilerplateCollection,
        mapper), IBoilerplateRepository;