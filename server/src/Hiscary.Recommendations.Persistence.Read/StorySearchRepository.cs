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
    private readonly IUserPreferencesReadRepository _userPreferencesReadRepository;

    public StorySearchRepository(
        ElasticsearchClient client,
        ElasticsearchConfiguration settings,
        IUserPreferencesReadRepository userPreferencesReadRepository)
    {
        _client = client;
        _settings = settings;
        _userPreferencesReadRepository = userPreferencesReadRepository;
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
                    new TermQuery(Infer.Field<Story>(f => f.LibraryId), FieldValue.String(query.LibraryId.ToString())),
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
            response.Total,
            response.Documents.Count
        );
    }

    public async Task<ClientQueriedModel<Story>> RecommendationsForUser(StoryRecommendationsQuery query, CancellationToken ct = default)
    {
        var userPreferences = await _userPreferencesReadRepository.GetByIdAsync(query.UserAccountId, ct);

        SearchResponse<Story> response;

        if (userPreferences is not null &&
            (userPreferences.FavoriteGenres?.Length > 0 || userPreferences.FavoriteTags?.Length > 0))
        {
            response = await _client.SearchAsync<Story>(s => s
                .Indices(_settings.StoryIndex)
                .From(query.StartIndex)
                .Size(query.ItemsCount)
                .Query(q => q
                    .Bool(b => b
                        .Should(sh =>
                        {
                            var tags = userPreferences.FavoriteTags?.Where(w => w.Length > 5)?.TakeLast(2)?.ToArray() ?? [];
                            if (tags.Length > 0)
                            {
                                sh.Match(m => m
                                    .Field(new Field("title"))
                                    .Query(string.Join(" ", tags))
                                );

                                sh.Match(m => m
                                    .Field(new Field("description"))
                                    .Query(string.Join(" ", tags))
                                );
                            }
                        })
                        .Should(sh =>
                        {
                            if (userPreferences.FavoriteGenres?.Length > 0)
                            {
                                sh.Terms(new TermsQuery
                                {
                                    Field = new Field("genres.keyword"),
                                    Terms = new TermsQueryField(
                                        userPreferences.FavoriteGenres
                                            .Select(FieldValue.String)
                                            .ToArray()
                                    )
                                });
                            }
                        })
                        .MinimumShouldMatch(1)
                    )
                )
                .Sort(srt => srt
                    .Field(new FieldSort (new Field("uniqueReads")) { Order = SortOrder.Desc })
                )
            , ct);
        }
        else
        {
            response = await _client.SearchAsync<Story>(s => s
                .Indices(_settings.StoryIndex)
                .From(query.StartIndex)
                .Size(query.ItemsCount)
                .Query(q => q.MatchAll(new MatchAllQuery()))
                .Sort(srt => srt
                    .Field(new FieldSort (new Field("uniqueReads")) { Order = SortOrder.Desc })
                )
            , ct);
        }

        if (!response.IsValidResponse || !response.IsSuccess())
        {
            return ClientQueriedModel<Story>.Empty;
        }

        return ClientQueriedModel<Story>.Create(
            response.Documents.ToList(),
            response.Total,
            response.Documents.Count
        );
    }
}
