using Api.Common.DomainModels.Responses.Crud;
using Api.Common.Entities;
using Api.Common.Models;
using AutoMapper;
using MongoDB.Driver;

namespace Api.Common.Repositories;

public abstract class CrudRepository<TEntity, TModel> : ICrudRepository<TEntity, TModel>
    where TEntity : IdentifiableEntity
    where TModel : IdentifiableModel
{
    private readonly IMongoCollection<TEntity>? _collection;
    private readonly IMapper _mapper;

    private readonly List<string> _ctorErrors = [];

    protected CrudRepository(string connectionString, string databaseName, string collectionName, IMapper mapper)
    {
        _mapper = mapper;

        try
        {
            var client = new MongoClient(connectionString);
            var database = client.GetDatabase(databaseName);

            if (!CollectionExists(database, collectionName))
                throw new InvalidOperationException($"MongoDB collection '{collectionName}' does not exist.");

            _collection = database.GetCollection<TEntity>(collectionName);
        }
        catch (Exception ex)
        {
            _ctorErrors.Add($"Failed to initialize MongoDB repository: {ex.Message}");
            Console.WriteLine("## Failed to initialize MongoDB repository");
            Console.WriteLine($"## Message : {ex.Message}{Environment.NewLine}{ex.StackTrace}");
        }
    }

    private static bool CollectionExists(IMongoDatabase database, string collectionName)
    {
        try
        {
            var collections = database.ListCollectionNames().ToList();
            return collections.Contains(collectionName);
        }
        catch
        {
            return false;
        }
    }

    public async Task<CrudResponse<IEnumerable<TModel>>> AddMany(IList<TModel> models)
    {
        if (!ValidateContext<IEnumerable<TModel>>(out var response))
            return response;

        if (!models.Any())
        {
            response.Errors.Add("There is no elements to sended");
            return response;
        }

        try
        {
            var model = _mapper.Map<IEnumerable<TEntity>>(models);
            await _collection!.InsertManyAsync(model);
        }
        catch (Exception e)
        {
            response.Errors.Add(e.Message);
        }

        response.Data = models;
        return response;
    }

    public async Task<CrudResponse<TModel>> Update(string id, TModel model)
        => !ValidateContext<TModel>(out var response)
            ? response
            : _mapper.Map<CrudResponse<TModel>>(await _collection.ReplaceOneAsync(x => x.Id == id, _mapper.Map<TEntity>(model))).ValidateResult();

    public async Task<CrudResponse<IEnumerable<TModel>>> UpdateMany(IEnumerable<TModel> models)
    {
        if (!ValidateContext<IEnumerable<TModel>>(out var response))
            return response;

        try
        {
            var updates = new List<WriteModel<TEntity>>();
            var entities = _mapper.Map<IEnumerable<TEntity>>(models);

            foreach (var doc in entities)
            {
                var filterById = Builders<TEntity>.Filter.Eq(d => d.Id, doc.Id);
                updates.Add(new ReplaceOneModel<TEntity>(filterById, doc));
            }

            if (updates.Any())
            {
                var bulkResult = await _collection!.BulkWriteAsync(updates);

                return new CrudResponse<IEnumerable<TModel>>
                {
                    IsAcknowledged = true,
                    MatchedCount = bulkResult.MatchedCount,
                    ModifiedCount = bulkResult.ModifiedCount
                };
            }

            return new CrudResponse<IEnumerable<TModel>>
            {
                MatchedCount = 0,
                ModifiedCount = 0,
                IsAcknowledged = false,
                Data = models
            };
        }
        catch (Exception ex)
        {
            return response.ThrowException(ex.Message);
        }
    }

    public async Task<CrudResponse<TModel>> Upsert(string? id, TModel model)
    {
        if (!ValidateContext<TModel>(out var response))
            return response;

        if (id is null)
        {
            var createdEntity = await Add(model);
            createdEntity.UpsertedId = createdEntity.Data?.Id;
            return createdEntity;
        }

        return _mapper.Map<CrudResponse<TModel>>(await _collection.ReplaceOneAsync(x => x.Id == id, _mapper.Map<TEntity>(model))).ValidateResult();
    }

    public async Task<CrudResponse<IEnumerable<TModel>>> GetAll(int? startIndex = null, int? count = null, List<string>? fieldsToExclude = null)
    {
        if (!ValidateContext<IEnumerable<TModel>>(out var response))
            return response;

        try
        {
            var filter = Builders<TEntity>.Filter.Empty;
            var query = _collection.Find(filter);

            if (fieldsToExclude != null && fieldsToExclude.Any())
            {
                var projectionBuilder = Builders<TEntity>.Projection;
                var projection = projectionBuilder.Exclude(fieldsToExclude.First());

                foreach (var field in fieldsToExclude.Skip(1))
                    projection = projection.Exclude(field);

                query = query.Project<TEntity>(projection);
            }

            if (startIndex.HasValue)
                query = query.Skip(startIndex.Value);

            if (count.HasValue)
                query = query.Limit(count.Value);

            var result = await query.ToListAsync();
            response.MatchedCount = result?.Count;
            response.Data = _mapper.Map<IEnumerable<TModel>>(result);
            return response;
        }
        catch (Exception ex)
        {
            return response.ThrowException(ex.Message);
        }
    }

    public async Task<CrudResponse<TModel?>> GetById(string id)
    {
        if (!ValidateContext<TModel?>(out var response))
            return response;

        try
        {
            var entity = (await _collection.FindAsync(x => x.Id == id)).FirstOrDefault();
            response.MatchedCount = entity != null ? 1 : 0;
            response.Data = _mapper.Map<TModel>(entity);
            return response;
        }
        catch (Exception ex)
        {
            return response.ThrowException(ex.Message);
        }
    }

    public async Task<CrudResponse<TModel>> Add(TModel model)
    {
        if (!ValidateContext<TModel>(out var response))
            return response;

        try
        {
            var entity = _mapper.Map<TEntity>(model);
            await _collection!.InsertOneAsync(entity);
            response.IsAcknowledged = true;
            response.CreatedCount = 1;
            response.Data = model;
            return response;
        }
        catch (Exception ex)
        {
            return response.ThrowException(ex.Message);
        }
    }

    public async Task<CrudResponse<TModel>> Delete(string id)
    {
        if (!ValidateContext<TModel>(out var response))
            return response;

        try
        {
            var model = (await GetById(id)).Data;

            if (model is null)
                return new CrudResponse<TModel> { IsAcknowledged = true, ModifiedCount = 0, MatchedCount = 0 };

            model.Deleted = true;

            return _mapper.Map<CrudResponse<TModel>>(await _collection.ReplaceOneAsync(x => x.Id == id, _mapper.Map<TEntity>(model))).ValidateResult();
        }
        catch (Exception ex)
        {
            return response.ThrowException(ex.Message);
        }
    }

    public async Task<CrudResponse<IEnumerable<TModel>>> UpgradeData()
    {
        var entities = (await _collection.FindAsync(x => true)).ToList();
        var models = _mapper.Map<IEnumerable<TModel>>(entities);
        return await UpdateMany(models);
    }

    private bool ValidateContext<T>(out CrudResponse<T> response)
    {
        response = new CrudResponse<T>();
        if (_ctorErrors.Any())
        {
            response.Errors = _ctorErrors;
            return false;
        }

        if (_collection is null)
        {
            response.Errors.Add("The collection is not initialized");
            return false;
        }

        return true;
    }
}