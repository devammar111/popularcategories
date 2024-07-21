using PopularCategoriesGenerator.Models;

namespace PopularCategoriesGenerator.Services
{
	public interface IAiApiService
	{
		 Task<List<OutputAttribute>> GetPopularAttributesAsync(List<Category> categories);
	}
}
