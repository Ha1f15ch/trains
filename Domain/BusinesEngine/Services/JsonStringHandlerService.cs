using Newtonsoft.Json;

namespace BusinesEngine.Services
{
    public class JsonStringHandlerService
    {
        private readonly JsonSerializerSettings _jsonSerializeSettings;

		public JsonStringHandlerService()
		{
            _jsonSerializeSettings = new JsonSerializerSettings
            {
                Formatting = Formatting.Indented // форматирование в привычный вид выводимой строки
            };
		}

        //Преобразование строки в объект json (лист/массив)
		public Task<string> SerializeList<T>(List<T> items) => Task.Run(() => 
        { 
            if(items.Count == 0)
            {
                throw new ArgumentNullException("Для сериализации объектов переданы некорректные данные");
            }

            return JsonConvert.SerializeObject(items, _jsonSerializeSettings); 
        });

		//Преобразование строки в объект json (один элемент)
		public Task<string> SerializeSingle<T>(T item) => Task.Run(() => 
        {
			if (item is null)
			{
				throw new NullReferenceException("Для сериализации объектов переданы некорректные данные");
			}

			return JsonConvert.SerializeObject(item, _jsonSerializeSettings); 
        });
    }
}
