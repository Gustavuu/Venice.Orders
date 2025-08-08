namespace Venice.Orders.API.Token
{
    public interface ITokenService
    {
        string GenerateToken(string userId);
    }
}
