using LW3.Models;

namespace LW3.Repositories;

public interface IPartRepository : IRepository<Part>
{
    // Атомарно списує кількість зі складу, якщо залишку достатньо. Повертає false, якщо залишку не вистачило.
    Task<bool> TryReserveAsync(string id, int quantity);

    // Повертає раніше списану кількість на склад (компенсація при помилці).
    Task ReleaseAsync(string id, int quantity);
}
