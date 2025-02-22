using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

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

		public Task<string> SerializeList<T>(List<T> items) => Task.Run(() => 
        { 
            if(items.Count == 0)
            {
                throw new ArgumentNullException("Для реализации объектов переданы некорректные данные");
            }

            return JsonConvert.SerializeObject(items, _jsonSerializeSettings); 
        });

		public Task<string> SerializeSingle<T>(T item) => Task.Run(() => 
        {
			if (item is null)
			{
				throw new NullReferenceException("Для реализации объектов переданы некорректные данные");
			}

			return JsonConvert.SerializeObject(item, _jsonSerializeSettings); 
        });
    }
}
