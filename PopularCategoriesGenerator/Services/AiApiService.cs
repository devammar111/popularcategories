using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using PopularCategoriesGenerator.Models;
namespace PopularCategoriesGenerator.Services
{

	public class AiApiService : IAiApiService
	{
		private readonly HttpClient _httpClient;

		public AiApiService(HttpClient httpClient)
		{
			_httpClient = httpClient;
		}

		public async Task<List<OutputAttribute>> GetPopularAttributesAsync(List<Category> categories)
		{
			var attributesResults = new List<OutputAttribute>();

			foreach (var category in categories)
			{
				foreach (var subCategory in category.SubCategories)
				{
					var attributes = await FetchAttributesFromAI(subCategory.CategoryName);
					attributesResults.Add(new OutputAttribute
					{
						CategoryId = subCategory.CategoryId,
						Attributes = attributes
					});
				}
			}

			return attributesResults;
		}

		private async Task<List<string>> FetchAttributesFromAI(string categoryName)
		{
			// Replace with your AI API endpoint and API key
			var apiUrl = "https://api.openai.com/v1/engines/davinci/completions"; // Example OpenAI endpoint
			var apiKey = ""; // Replace Your API Key here, you can fetch from here https://platform.openai.com/api-keys

			var requestBody = new
			{
				prompt = $"Get 3 popular attributes for the category: {categoryName}",
				max_tokens = 50
			};

			var requestContent = new StringContent(JsonSerializer.Serialize(requestBody), Encoding.UTF8, "application/json");

			_httpClient.DefaultRequestHeaders.Clear();
			_httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {apiKey}");

			try
			{
				var response = await _httpClient.PostAsync(apiUrl, requestContent);

				if (response.IsSuccessStatusCode)
				{
					var jsonResponse = await response.Content.ReadAsStringAsync();
					var aiResponse = JsonSerializer.Deserialize<AIResponse>(jsonResponse);
					return aiResponse.Choices[0].Text.Split(new[] { '\n', ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();
				}
				else
				{
					var errorResponse = await response.Content.ReadAsStringAsync();
					Console.WriteLine($"Error: {response.StatusCode}, Details: {errorResponse}");
					return new List<string> { "Error fetching attributes" };
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine($"Exception: {ex.Message}");
				return new List<string> { "Exception occurred" };
			}
		}
	}

	public class AIResponse
	{
		public List<Choice> Choices { get; set; }
	}

	public class Choice
	{
		public string Text { get; set; }
	}
}
