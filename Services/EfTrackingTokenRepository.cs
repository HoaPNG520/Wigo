using Microsoft.EntityFrameworkCore;
using Wigo.Data;
using Wigo.Models.Entities;
using Wigo.Models.ValueObjects;

namespace Wigo.Services;

public class EfTrackingTokenRepository : ITrackingTokenRepository
{
    private readonly UiiDbContext _context;

    public EfTrackingTokenRepository(UiiDbContext context)
    {
        _context = context;
    }

    public async Task<TrackingToken?> GetByValueAsync(string value)
    {
        return await _context.TrackingTokens.FirstOrDefaultAsync(t => t.Value == value);
    }

    public async Task<TrackingToken> AddAsync(TrackingToken token)
    {
        _context.TrackingTokens.Add(token);
        await _context.SaveChangesAsync();
        return token;
    }
}