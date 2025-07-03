namespace ExamenPOO.Core.Interfaces;

public interface IJwtService
{
    string GenerateToken(int userId, string username);
    int? ValidateToken(string token);
}
