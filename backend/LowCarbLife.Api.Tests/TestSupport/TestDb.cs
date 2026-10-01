using LowCarbLife.Api.Data;
using Microsoft.EntityFrameworkCore;

namespace LowCarbLife.Api.Tests.TestSupport;

/// <summary>
/// Izolowana baza in-memory. Każde wywołanie <see cref="NewContext"/> zwraca nowy kontekst
/// na tych samych danych, dzięki czemu testy sprawdzają faktyczny zapis (a nie cache śledzenia zmian).
/// </summary>
internal sealed class TestDb
{
    private readonly string _name = Guid.NewGuid().ToString();

    public AppDbContext NewContext() =>
        new(new DbContextOptionsBuilder<AppDbContext>()
            .UseInMemoryDatabase(_name)
            .Options);
}
