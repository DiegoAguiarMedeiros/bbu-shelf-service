
namespace Api.Contracts;

public record UpdateShelfRequest(
    string Name,
    string PersonaName,
    string CustomURL,
    string Bio
    );
