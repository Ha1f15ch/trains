using WebAppTrain.Repositories.Intefaces;

namespace WebAppTrain.Repositories.Repository
{
    public class ExampleRepository : IExampleRepository
    {
        private readonly ILogger<ExampleRepository> _logger;

        public ExampleRepository(ILogger<ExampleRepository> logger)
        {
            _logger = logger;
        }

        public IEnumerable<string> GetItems()
        {
            _logger.LogInformation("Вызван метод {MethodName}", nameof(GetItems));
            return new List<string> { "Item1", "Item2", "Item3" };
        }

        public string GetItemById(int id)
        {
            _logger.LogInformation("Вызван метод - {MethodName} с аргументом {id}", nameof(GetItemById), id);
            return $"Item{id}";
        }
    }
}