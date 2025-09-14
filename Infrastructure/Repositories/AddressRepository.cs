
using Infrastructure.Contexts;
using Infrastructure.Entities;

namespace Infrastructure.Repositories
{
    public class AddressRepository(DataContext context) : Repo<DataContext, AddressEntity>(context)
    {
    }
}
