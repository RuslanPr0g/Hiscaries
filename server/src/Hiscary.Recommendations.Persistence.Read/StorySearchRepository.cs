using Elastic.Clients.Elasticsearch;
using Elastic.Clients.Elasticsearch.QueryDsl;
using Hiscary.Recommendations.Domain.Entities;
using Hiscary.Recommendations.Domain.Persistence.Read;
using Hiscary.Recommendations.Domain.Queries;
using Hiscary.Recommendations.Persistence.Shared;
using StackNucleus.DDD.Domain.ClientModels;
using StackNucleus.DDD.Persistence;

namespace Hiscary.Recommendations.Persistence.Read;

public class StorySearchRepository : IStorySearchRepository
{
    private readonly ElasticsearchClient _client;
    private readonly ElasticsearchConfiguration _settings;

    public StorySearchRepository(ElasticsearchClient client, ElasticsearchConfiguration settings)
    {
        _client = client;
        _settings = settings;
    }

    public async Task<Story?> GetByIdAsync(Guid id, CancellationToken ct = default)
    {
        var response = await _client.GetAsync<Story>(id, g => g.Index(_settings.StoryIndex), ct);
        return response.Source;
    }

    public async Task<ClientQueriedModel<Story>> SearchAsync(SearchStoryQuery query, CancellationToken ct = default)
    {
        if (string.IsNullOrWhiteSpace(query.Title))
        {
            return ClientQueriedModel<Story>.Empty;
        }

        var searchRequest = new SearchRequest(_settings.StoryIndex)
        {
            From = query.StartIndex,
            Size = query.ItemsCount,
            Query = new BoolQuery
            {
                Must = new List<Query>
                {
                    new MultiMatchQuery
                        {
                            Query = query.Title,
                            Fields = Infer.Fields<Story>(f => f.Title, f => f.Description),
                            Type = TextQueryType.BestFields,
                            TieBreaker = 0.3,
                            Boost = (float)1.0,
                        }
                }.Where(q => q != null).ToList(),
                Filter = new List<Query>
                {
                    new TermQuery(Infer.Field<Story>(f => f.LibraryId))
                    {
                        Value = FieldValue.String(query.LibraryId.ToString())
                    },
                    new DateRangeQuery(Infer.Field<Story>(f => f.PublishedDate))
                    {
                        Gte = query.PublishedDate.Date,
                        Lt = query.PublishedDate.Date.AddDays(1)
                    }
                }.SelectSkipNulls(x => x).Where(q => q != null).ToList()
            }
        };

        var response = await _client.SearchAsync<Story>(searchRequest, ct);

        return ClientQueriedModel<Story>.Create(
            response.Documents.ToList(),
            (int)response.Total
        );
    }


    private static async Task PopulateMockStories(
        ElasticsearchClient client,
        CancellationToken cancellationToken)
    {
        var createIndexResponse = await client.Indices.CreateAsync("stories", cancellationToken);

        var story = new Story
        {
            Id = Guid.NewGuid(),
            Title = "The Lost Chronicles",
            Description = "An epic tale of adventure, mystery, and forgotten kingdoms.",
            Genres = new HashSet<string> { "Fantasy", "Adventure", "Mystery" },
            LibraryId = Guid.NewGuid(),
            PublishedDate = new DateTime(2023, 5, 14)
        };

        var indexStoryResponse = await client.IndexAsync(story, x => x.Index("stories"));

        var searchForCreatedStoryNotFound = await client.SearchAsync<Story>(s => s
            .Indices("stories")
            .From(0)
            .Size(10)
            .Query(q => q
                .Term(t => t
                    .Field(x => x.Title)
                    .Value("cringe")
                )
            )
        );

        if (searchForCreatedStoryNotFound.IsValidResponse)
        {
            var doc = searchForCreatedStoryNotFound.Documents.FirstOrDefault();
        }

        var searchForCreatedStoryFound = await client.SearchAsync<Story>(s => s
            .Indices("stories")
            .From(0)
            .Size(10)
            .Query(q => q
                .Term(t => t
                    .Field(x => x.Title)
                    .Value("lost")
                )
            )
        );

        if (searchForCreatedStoryFound.IsValidResponse)
        {
            var doc = searchForCreatedStoryFound.Documents.FirstOrDefault();
        }
    }

    public Task<ClientQueriedModel<Story>> RecommendationsForUser(Guid userId, CancellationToken ct = default)
    {
        throw new NotImplementedException();
    }
}
