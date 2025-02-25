namespace CommonInterfaces.ApiClients
{
	public interface IApiClient
	{
		Task<T?> GetAsync<T>(string url);
		Task<TResponse?> PostAsync<TRequest, TResponse>(string url, TRequest data);
	}
}
