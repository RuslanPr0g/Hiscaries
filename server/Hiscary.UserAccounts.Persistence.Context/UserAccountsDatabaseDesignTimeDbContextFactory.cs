﻿using StackNucleus.DDD.Persistence.EF.Postgres;
using Microsoft.EntityFrameworkCore;

namespace Hiscary.UserAccounts.Persistence.Context;

public class UserAccountsDatabaseDesignTimeDbContextFactory
    : NucleusDatabaseDesignTimeDbContextFactory<UserAccountsContext>
{
    public override UserAccountsContext CreateDbContextBasedOnOptions(
        DbContextOptions<UserAccountsContext> options)
    {
        return new UserAccountsContext(options);
    }
}
