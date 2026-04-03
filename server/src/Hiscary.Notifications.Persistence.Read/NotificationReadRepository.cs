using Hiscary.Notifications.Domain.DataAccess;
using Hiscary.Notifications.Domain.ReadModels;
using Hiscary.Notifications.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using StackNucleus.DDD.Persistence.EF.Postgres;

namespace Hiscary.Notifications.Persistence.Read;

public class NotificationReadRepository(NotificationsContext context) :
    BaseReadRepository<NotificationReadModel>,
    INotificationReadRepository
{
    private NotificationsContext Context { get; init; } = context;

    public async Task<NotificationReadModel?> GetById(Guid id)
    {
        var result = await Context.Notifications
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id);

        if (result is null)
        {
            return null;
        }

        return NotificationReadModel.FromDomainModel(result);
    }

    public async Task<IEnumerable<NotificationReadModel>> GetMissedNotificationsByUserId(Guid userAccountId) =>
        await Context.Notifications
            .AsNoTracking()
            .Where(x => x.UserId == userAccountId && x.IsRead == false)
            .OrderByDescending(x => x.CreatedAt)
            .Select(user => NotificationReadModel.FromDomainModel(user))
            .ToListAsync();

    public async Task<IEnumerable<NotificationReadModel>> GetNotificationsByUserId(Guid userAccountId) =>
        await Context.Notifications
            .AsNoTracking()
            .Where(x => x.UserId == userAccountId)
            .OrderByDescending(x => x.CreatedAt)
            .Select(user => NotificationReadModel.FromDomainModel(user))
            .ToListAsync();
}

