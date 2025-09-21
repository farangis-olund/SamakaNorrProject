
using Infrastructure.Contexts;
using Infrastructure.Entities;

namespace Infrastructure.Repositories
{
    public class NotificationRepository(DataContext context) : Repo<DataContext, NotificationEntity>(context)
    {
    }
}
