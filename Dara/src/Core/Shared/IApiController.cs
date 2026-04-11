using Dara.Core.Shared.Services;

namespace Dara.Core.Shared;

public interface IApiController<T> where T : IDaraService;
