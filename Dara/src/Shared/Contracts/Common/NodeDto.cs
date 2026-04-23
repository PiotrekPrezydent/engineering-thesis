using Dara.Shared.Contracts.Abstractions;

namespace Dara.Shared.Contracts.Common;

public record NodeDto(string Name, string AuthCode) : IAppResponse;

public record NodeNameDto(string Name);

public record NodeAuthCodeDto(string AuthCode);