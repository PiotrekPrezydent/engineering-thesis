using Dara.Shared.Contracts.Connection;

namespace Dara.Shared.Contracts.Communication;

public record ResponseDto(ResponseType ResponseType, MessageDto Message);