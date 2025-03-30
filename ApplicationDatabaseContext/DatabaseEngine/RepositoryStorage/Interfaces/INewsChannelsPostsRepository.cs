using DatabaseEngine.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DatabaseEngine.RepositoryStorage.Interfaces
{
    public interface INewsChannelsPostsRepository
    {
        public Task<NewsChannelsPosts?> CreateNewNewsChannelsPost(int newChannelId, string titlePost, string bodyPost, string? footerPost, string authorPost, string? sourceImage);
        public Task<List<NewsChannelsPosts>> GetAllPosts();
        //Если нужно получить все посты по Id новостного канала
        public Task<List<NewsChannelsPosts>> GetAllPostsByNewsChannelId(int newsChannelId);
        //Если нужно будет выполнять частичный поиск по титульному заголовку поста
        public Task<List<NewsChannelsPosts>> GetAllPostsByPartTitle(string titlePost);
    }
}
