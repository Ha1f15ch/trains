using DatabaseEngine.Models;
using DatabaseEngine.RepositoryStorage.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace DatabaseEngine.RepositoryStorage.Repositories
{
    public class NewsChannelsPostsRepository : INewsChannelsPostsRepository
    {
        private readonly AppDbContext _appDbContext;

        public NewsChannelsPostsRepository(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        //создаем новый пост из новостного канала 
        public async Task<NewsChannelsPosts?> CreateNewNewsChannelsPost(int newChannelId, string titlePost, string bodyPost, string? footerPost, string authorPost, string? sourceImage)
        {
            try
            {
                //Проверяем на наличие записи в БД по id канала
				var newsChannel = await _appDbContext.NewsChannels.FindAsync(newChannelId);

				if (newsChannel is null)
				{
					Console.WriteLine($"По заданному id канала, найти такой канал не удалось");
					return null;
				}

				//Проверяем на null authorPost
				if (string.IsNullOrEmpty(authorPost))
				{
					Console.WriteLine($"Параметр authorPost не может быть null");
					return null;
				}

				//Проверяем на null bodyPost
				if (string.IsNullOrEmpty(bodyPost))
                {
					Console.WriteLine($"Параметр bodyPost не может быть null");
					return null;
				}

				//Проверяем на null titlePost
				if (string.IsNullOrEmpty(titlePost))
                {
					Console.WriteLine($"Параметр titlePost не может быть null");
					return null;
				}

				//Создаем экземпляр новой записи для сохранения ее в контексте 
				var newPost = new NewsChannelsPosts
				{
					NewsChannelId = newsChannel.Id,
					TitlePost = titlePost,
					BodyPost = bodyPost,
					FooterPost = footerPost,
					AuthorPost = authorPost,
					SurceImage = sourceImage,
					CreatedDate = DateTime.UtcNow
				};

				await _appDbContext.NewsChannelsPosts.AddAsync(newPost);
				await _appDbContext.SaveChangesAsync();

				Console.WriteLine($"Запись новостного сообщения была создана. newPost = {nameof(newPost)}. Где newPost.NewsChannelId = {newPost.NewsChannelId}\nnewPost.TitlePost = {newPost.TitlePost}\nnewPost.BodyPost = {newPost.BodyPost}\nnewPost.FooterPost = {newPost.FooterPost}\nnewPost.AuthorPost = {newPost.AuthorPost}\nnewPost.SurceImage = {newPost.SurceImage}\nnewPost.CreatedDate = {newPost.CreatedDate}");

				return newPost;
			}
            catch( Exception ex )
            {
				Console.WriteLine($"При выполнении метода {nameof(CreateNewNewsChannelsPost)} возникла ошибка: {ex.Message}");
				return null;
			}
        }

        public async Task<List<NewsChannelsPosts>> GetAllPosts()
        {
			try
			{
				var posts = await _appDbContext.NewsChannelsPosts.ToListAsync();
				return posts;
			}
			catch (Exception ex)
			{
				Console.WriteLine($"При выполнении метода {nameof(GetAllPosts)} возникла ошибка: {ex.Message}");
				return new List<NewsChannelsPosts>();
			}
		}

        public async Task<List<NewsChannelsPosts>> GetAllPostsByNewsChannelId(int newsChannelId)
        {
            try
            {
				Console.WriteLine($"Поиск по id новостного канала");
                var newsChannel = await _appDbContext.NewsChannels.FindAsync(newsChannelId);

				//Проверка на null
                if(newsChannel is null)
                {
                    Console.WriteLine($"Передано некорректное значение id новостного канала - {newsChannelId} - {nameof(newsChannelId)}. Не найден новостной канал по значению id - {newsChannelId}");
					return new List<NewsChannelsPosts>();
				}

                Console.WriteLine($"Поиск постов для новостного канала - {newsChannel} - {nameof(newsChannel)}");
				
				var listPosts = await _appDbContext.NewsChannelsPosts
					.Where(el => el.NewsChannelId == newsChannelId)
					.ToListAsync();

				//Проверяем, есть ли посты
				if (!listPosts.Any())
				{
					Console.WriteLine($"Постов от новостного канала с id = {newsChannelId} не найдено.");
					return new List<NewsChannelsPosts>();
				}

				return listPosts;
			}
            catch(Exception ex)
            {
                Console.WriteLine($"Возникла ошибка при поиске постов для новостного канала id = {newsChannelId} - {ex.Message}");
				return new List<NewsChannelsPosts>();
			}
        }

        public async Task<List<NewsChannelsPosts>> GetAllPostsByPartTitle(string titlePost)
        {
			try
            {
				if (string.IsNullOrEmpty(titlePost))
				{
					Console.WriteLine($"Некорректное значение titlePost поста новостного канала: {titlePost}. Ожидается не null.");
					return new List<NewsChannelsPosts>();
				}

				Console.WriteLine($"Поиск постов по заголовку = {titlePost}, вызван метод {nameof(GetAllPostsByPartTitle)}");
				var listPostByPartTitlePost = await _appDbContext.NewsChannelsPosts
					.Where(el => el.TitlePost.ToLower().Contains(titlePost.ToLower()))
					.ToListAsync();

				if (!listPostByPartTitlePost.Any())
                {
                    Console.WriteLine($"Не найдено совпадений с заданной частью заголовка - {titlePost}");
					return new List<NewsChannelsPosts>();
				}

				//Если нашли
				Console.WriteLine($"Найдено {listPostByPartTitlePost.Count} постов с частью заголовка '{titlePost}'.");

				return listPostByPartTitlePost;
			}
            catch (Exception ex)
            {
                Console.WriteLine($"При выполнении метода {nameof(GetAllPostsByPartTitle)} возникла ошибка: {ex.Message}");
				return new List<NewsChannelsPosts>();
			}
        }
    }
}
