using Dara.Server.BuildingBlocks.Domain;

namespace Dara.Server.Modules.Identity.Domain.Sessions;

public interface IDeviceSessionRepository : IRepository
{
    Task AddAsync(DeviceSession session);

    Task<DeviceSession?> FindByIdAsync(DeviceId id);

    // any token of the session, used or not, so a reuse is recognized
    Task<DeviceSession?> FindByTokenHashAsync(string tokenHash);

    // the device whose open code has this hash
    Task<DeviceSession?> FindByLoginCodeHashAsync(string codeHash);
}
