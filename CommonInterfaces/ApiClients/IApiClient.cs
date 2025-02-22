namespace CommonInterfaces.ApiClients
{
	public interface IApiClient
	{
		Task<T?> GetAsync<T>(string url);
		Task<T?> PostAsync<T>(string url, T data);
	}
}
