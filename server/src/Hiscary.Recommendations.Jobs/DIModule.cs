using Microsoft.Extensions.DependencyInjection;

namespace Hiscary.Recommendations.Jobs;

public static class DIModule
{
    public static IServiceCollection AddJobs(this IServiceCollection services)
    {
        //return services.AddConfigurableJobs([
        //    new JobConfiguration
        //    {
        //        Key = nameof(IndexStoriesJob),
        //        Type = typeof(IndexStoriesJob),
        //        RepeatInterval = 69,
        //        RepeatForever = true
        //    }
        //]);
        return services;
    }
}