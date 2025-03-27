namespace CommonDtoModels
{
	public class UpdatedBook
	{
		public int Id { get; set; }
		public string Name { get; set; }
		public string? Description { get; set; }
		public List<int> GenresId { get; set; }
	}
}
