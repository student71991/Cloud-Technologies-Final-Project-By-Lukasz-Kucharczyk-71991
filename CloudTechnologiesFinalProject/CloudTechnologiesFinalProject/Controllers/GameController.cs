using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Cosmos;
namespace CloudTechnologiesFinalProject.Controllers;
[Route("api/[controller]")]
[ApiController]
public class GameController(IConfiguration configuration, ILogger<GameController> logger) :
ControllerBase
{
    private readonly string _connectionString = configuration["COSMOS_CONNECTION_STRING"]!;
    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        logger.LogInformation("The GetAll action called using /api/game route");
        try
        {
            logger.LogInformation("Retrieving the data from the database");
            using CosmosClient client = new(_connectionString);
            var container = client.GetDatabase("CTFinalProject").GetContainer("Games");
            var query = new QueryDefinition("SELECT * FROM c");
            var iterator = container.GetItemQueryIterator<Game>(query);
            var games = new List<Game>();
            while (iterator.HasMoreResults)
            {
                var response = await iterator.ReadNextAsync();
                games.AddRange(response);
            }
            logger.LogInformation("Data retrieved successfully");
            return Ok(games);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Error while retrieving the data. See the exception details");
            throw;
        }
    }
}
public record Game(int Id, string GameTitle, string Developer);