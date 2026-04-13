namespace Hiscary.Shared.Domain.Extensions;

public static class TaskExtensions
{
    public static async Task<List<T>> WhenAllSequentialAsync<T>(this IEnumerable<Task<T>> tasks)
    {
        var results = new List<T>();

        foreach (var task in tasks)
        {
            results.Add(await task);
        }

        return results;
    }
}