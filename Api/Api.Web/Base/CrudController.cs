using Api.Common.DomainModels.Responses.Crud;
using Api.Common.Models;
using Api.Common.Services;
using Microsoft.AspNetCore.Mvc;

namespace Api.Web.Base;

public abstract class CrudController<T>(
    ICrudService<T> service,
    int defaultStartIndex = 0,
    int defaultCount = 99999,
    string? defaultFieldsToExclude = null) : BaseController
    where T : IdentifiableModel
{
    [HttpGet]
    public Task<IActionResult> GetAll(int? startIndex = null, int? count = null, string? fieldsToExclude = null)
        => HandleRequest(async () => Ok(await service.GetAll(startIndex ?? defaultStartIndex,
            count ?? defaultCount,
            (fieldsToExclude ?? defaultFieldsToExclude)?.Split(",").ToList())));

    [HttpGet("{id}")]
    public Task<IActionResult> GetById(string id)
        => HandleRequest(async () => await CheckIfExists(id) is { Data: not null } found ? Ok(found) : NotFound());

    [HttpPost]
    public Task<IActionResult> Post(T record)
        => HandleRequest(async () => CreatedAtAction(nameof(Post), await service.Add(record)));
    
    [HttpPost("Many")]
    public Task<IActionResult> Post(IList<T> records)
        => HandleRequest(async () => CreatedAtAction(nameof(Post), await service.AddMany(records)));

    [HttpPut]
    public Task<IActionResult> Put(T record)
        => HandleRequest(async () => record.Id != null && await CheckIfExists(record.Id) is not null ? Ok(await service.Update(record)) : NotFound());
    
    [HttpPut("Many")]
    public Task<IActionResult> Put(IList<T> records)
        => HandleRequest(async () => CreatedAtAction(nameof(Put), await service.UpdateMany(records)));

    [HttpDelete("{id}")]
    public Task<IActionResult> Delete(string id)
        => HandleRequest(async () => await CheckIfExists(id) is not null ? Ok(await service.Delete(id)) : NotFound());

    [HttpPost("Upgrade")]
    public Task<IActionResult> Upgrade()
        => HandleRequest(async () => Ok(await service.UpgradeData()));

    private async Task<CrudResponse<T>?> CheckIfExists(string id)
        => await service.GetById(id);
}