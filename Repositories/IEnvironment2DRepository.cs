public interface IEnvironment2DRepository
{
    Task<IEnumerable<Environment2D>> GetAllByUserIdAsync(string userId);
    Task<Environment2D?> GetByIdAsync(Guid id);
    Task<Guid> CreateAsync(Environment2D environment);
    Task DeleteAsync(Guid id);
    Task<int> GetCountByUserIdAsync(string userId);
}