using Hiscary.Notifications.Domain.ReadModels;
using StackNucleus.DDD.Domain.Repositories;

namespace Hiscary.Notifications.Domain.DataAccess;

public interface INotificationReadRepository : IBaseReadRepository<NotificationReadModel>
{
    Task<NotificationReadModel?> GetById(Guid id);
    Task<IEnumerable<NotificationReadModel>> GetMissedNotificationsByUserId(Guid userAccountId);
    Task<IEnumerable<NotificationReadModel>> GetNotificationsByUserId(Guid userAccountId);
}
