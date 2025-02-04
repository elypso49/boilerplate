using Api.Common.Dtos;
using Api.Services;
using Api.Web.Base;
using Microsoft.AspNetCore.Mvc;

namespace Api.Web.Controllers;

[ApiVersion("1.0")]
public class BoilerplateController(IBoilerplateService service) : CrudController<BoilerplateModel>(service)
{
    
}