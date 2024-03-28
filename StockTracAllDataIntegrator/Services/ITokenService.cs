namespace StockTracAllDataIntegrator.Services
{
    public interface ITokenService
    {
        Task<string> ExchangeAuthorizationCodeForToken(string authorizationCode, string redirectUri);
    }
}
