namespace BusinesEngine.Services.ServiceInterfaces
{
	public interface ILogService
	{
		/// <summary>
		/// Лог уровня информации.
		/// </summary>
		void LogInformation(string message);

		/// <summary>
		/// Лог уровня предупреждения.
		/// </summary>
		void LogWarning(string message);

		/// <summary>
		/// Лог уровня ошибки.
		/// </summary>
		void LogError(string message);

		/// <summary>
		/// Лог с уровнем отладочной информации.
		/// </summary>
		void LogDebug(string message);
	}
}
