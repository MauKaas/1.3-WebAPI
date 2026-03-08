using Dapper;

public class Environment2DRepository : IEnvironment2DRepository
{
    private readonly DapperContext _context;

    public Environment2DRepository(DapperContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Environment2D>> GetAllByUserIdAsync(string userId)
    {
        var query = "SELECT * FROM Environment2D WHERE UserId = @UserId";
        using var connection = _context.CreateConnection();
        return (await connection.QueryAsync<Environment2D>(query, new { UserId = userId })).ToList();
    }

    public async Task<Environment2D?> GetByIdAsync(Guid id)
    {
        var query = "SELECT * FROM Environment2D WHERE Id = @Id";
        using var connection = _context.CreateConnection();
        return await connection.QuerySingleOrDefaultAsync<Environment2D>(query, new { Id = id });
    }

    public async Task<Guid> CreateAsync(Environment2D environment)
    {
        environment.Id = Guid.NewGuid();
        var query = @"INSERT INTO Environment2D (Id, Name, MaxHeight, MaxLength, UserId) 
                      VALUES (@Id, @Name, @MaxHeight, @MaxLength, @UserId)";
        using var connection = _context.CreateConnection();
        await connection.ExecuteAsync(query, environment);
        return environment.Id;
    }

    public async Task DeleteAsync(Guid id)
    {
        var query = "DELETE FROM Environment2D WHERE Id = @Id";
        using var connection = _context.CreateConnection();
        await connection.ExecuteAsync(query, new { Id = id });
    }

    public async Task<int> GetCountByUserIdAsync(string userId)
    {
        var query = "SELECT COUNT(*) FROM Environment2D WHERE UserId = @UserId";
        using var connection = _context.CreateConnection();
        return await connection.ExecuteScalarAsync<int>(query, new { UserId = userId });
    }
}