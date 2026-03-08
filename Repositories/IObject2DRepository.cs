public interface IObject2DRepository
{
    Task<IEnumerable<Object2D>> GetAllByEnvironmentIdAsync(Guid environmentId);
    Task<Guid> CreateAsync(Object2D object2D);
    Task UpdateAsync(Object2D object2D);
    Task DeleteAsync(Guid id);
}