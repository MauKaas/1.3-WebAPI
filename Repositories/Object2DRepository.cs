using Dapper;

public class Object2DRepository : IObject2DRepository
{
    private readonly DapperContext _context;

    public Object2DRepository(DapperContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Object2D>> GetAllByEnvironmentIdAsync(Guid environmentId)
    {
        var query = "SELECT * FROM Object2D WHERE Environment2DId = @Environment2DId";
        using var connection = _context.CreateConnection();
        return (await connection.QueryAsync<Object2D>(query, new { Environment2DId = environmentId })).ToList();
    }

    public async Task<Guid> CreateAsync(Object2D object2D)
    {
        object2D.Id = Guid.NewGuid();
        var query = @"INSERT INTO Object2D (Id, PrefabId, PositionX, PositionY, ScaleX, ScaleY, RotationZ, SortingLayer, Environment2DId)
                      VALUES (@Id, @PrefabId, @PositionX, @PositionY, @ScaleX, @ScaleY, @RotationZ, @SortingLayer, @Environment2DId)";
        using var connection = _context.CreateConnection();
        await connection.ExecuteAsync(query, object2D);
        return object2D.Id;
    }

    public async Task UpdateAsync(Object2D object2D)
    {
        var query = @"UPDATE Object2D 
                      SET PrefabId = @PrefabId, PositionX = @PositionX, PositionY = @PositionY, 
                          ScaleX = @ScaleX, ScaleY = @ScaleY, RotationZ = @RotationZ, SortingLayer = @SortingLayer
                      WHERE Id = @Id";
        using var connection = _context.CreateConnection();
        await connection.ExecuteAsync(query, object2D);
    }

    public async Task DeleteAsync(Guid id)
    {
        var query = "DELETE FROM Object2D WHERE Id = @Id";
        using var connection = _context.CreateConnection();
        await connection.ExecuteAsync(query, new { Id = id });
    }
}