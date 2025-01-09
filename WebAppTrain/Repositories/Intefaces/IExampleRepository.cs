namespace WebAppTrain.Repositories.Intefaces
{
    public interface IExampleRepository
    {
        IEnumerable<string> GetItems();
        string GetItemById(int id);
    }
}
