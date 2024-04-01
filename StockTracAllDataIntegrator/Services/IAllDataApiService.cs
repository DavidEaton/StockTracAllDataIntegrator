namespace StockTracAllDataIntegrator.Services
{
    public interface IAllDataApiService
    {
        Task<string> GetCarComponentsAsync(string accessToken, int carId, int componentId, bool flatten);
    }
}
