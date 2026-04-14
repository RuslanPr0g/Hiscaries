using Hiscary.Stories.Domain.Stories;
using StackNucleus.DDD.Persistence.EF.Postgres.Configurations;

namespace Hiscary.Stories.Persistence.Context.Configurations;

public class CommentIdentityConverter : IdentityConverter<CommentId>
{
    public CommentIdentityConverter() :
        base((x) => new CommentId(x))
    {
    }
}
