using Dara.Server.Modules.Identity.Domain.Sessions;
using Microsoft.EntityFrameworkCore;

namespace Dara.Server.Modules.Identity.Infrastructure.Sessions;

public class DeviceSessionRepository : IDeviceSessionRepository
{
    private readonly IdentityContext _context;

    public DeviceSessionRepository(IdentityContext context)
    {
        _context = context;
    }

    public async Task AddAsync(DeviceSession session)
    {
        await _context.DeviceSessions.AddAsync(session);
    }

    public async Task<DeviceSession?> FindByIdAsync(DeviceId id)
    {
        return await _context.DeviceSessions.FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<DeviceSession?> FindByTokenHashAsync(string tokenHash)
    {
        return await _context.DeviceSessions.FirstOrDefaultAsync(s => s.Tokens.Any(t => t.TokenHash == tokenHash));
    }

    public async Task<DeviceSession?> FindByLoginCodeHashAsync(string codeHash)
    {
        return await _context.DeviceSessions.FirstOrDefaultAsync(s => s.LoginCode != null && s.LoginCode.Hash == codeHash);
    }
}
