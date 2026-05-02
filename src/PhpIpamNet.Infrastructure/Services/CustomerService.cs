using Microsoft.EntityFrameworkCore;
using PhpIpamNet.Domain.Entities;
using PhpIpamNet.Infrastructure.Data;

namespace PhpIpamNet.Infrastructure.Services;

public class CustomerService
{
    private readonly PhpIpamDbContext _db;
    public CustomerService(PhpIpamDbContext db) => _db = db;

    public Task<List<Customer>> ListAsync(CancellationToken ct = default)
        => _db.Customers.OrderBy(c => c.Title).ToListAsync(ct);

    public Task<Customer?> GetAsync(int id, CancellationToken ct = default)
        => _db.Customers.FirstOrDefaultAsync(c => c.Id == id, ct);

    public async Task<Customer> CreateAsync(Customer c, CancellationToken ct = default)
    {
        _db.Customers.Add(c);
        await _db.SaveChangesAsync(ct);
        return c;
    }

    public async Task UpdateAsync(Customer c, CancellationToken ct = default)
    {
        _db.Customers.Update(c);
        await _db.SaveChangesAsync(ct);
    }

    public async Task DeleteAsync(int id, CancellationToken ct = default)
    {
        var c = await _db.Customers.FindAsync(new object?[] { id }, ct);
        if (c is null) return;
        _db.Customers.Remove(c);
        await _db.SaveChangesAsync(ct);
    }
}
