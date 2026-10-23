using Wigo.Models.Entities;
using Wigo.Models.ValueObjects;

namespace Wigo.Services;

public interface ITrackingTokenRepository
{
    Task<TrackingToken?> GetByValueAsync(string value);
    Task<TrackingToken> AddAsync(TrackingToken token);
}