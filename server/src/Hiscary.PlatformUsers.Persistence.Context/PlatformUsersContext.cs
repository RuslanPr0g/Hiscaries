using Hiscary.PlatformUsers.Domain;
using Microsoft.EntityFrameworkCore;
using StackNucleus.DDD.Persistence.EF.Postgres;
using System.Reflection;

namespace Hiscary.PlatformUsers.Persistence.Context;

public sealed class PlatformUsersContext(DbContextOptions<PlatformUsersContext> options)
    : BaseNucleusContext<PlatformUsersContext>(options)
{

    public static string SCHEMA_NAME = "platformusers";

    public override string SchemaName => SCHEMA_NAME;

    public DbSet<PlatformUser> PlatformUsers { get; set; }
    public DbSet<Library> Libraries { get; set; }
    public DbSet<PlatformUserToLibrarySubscription> PlatformUserToLibrarySubscriptions { get; set; }
    public DbSet<UserAnnotatedPdf> UserAnnotatedPdfs { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
    }
}
