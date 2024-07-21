using Microsoft.AspNetCore.Mvc;
using PopularCategoriesGenerator.Models;
using PopularCategoriesGenerator.Services;

namespace PopularCategoriesGenerator.Controllers
{
	[ApiController]
	[Route("[controller]")]
	public class CategoryController : ControllerBase
	{
		private readonly AiApiService _aiApiService;

		public CategoryController(AiApiService aiApiService)
		{
			_aiApiService = aiApiService;
		}

		[HttpPost(Name = "GenerateAICategories")]
		public async Task<ActionResult<List<OutputAttribute>>> ProcessCategories([FromBody] List<Category> categories)
		{
			try
			{
				var attributes = await _aiApiService.GetPopularAttributesAsync(categories);
				return Ok(attributes);
			}
			catch (HttpRequestException)
			{
				return StatusCode(500, "Error calling AI API");
			}
		}
	}
}
