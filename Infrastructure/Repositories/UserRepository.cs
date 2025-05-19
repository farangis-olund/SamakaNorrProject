
using Infrastructure.Contexts;
using Infrastructure.Entities;


namespace Infrastructure.Repositories
{
	public class UserRepository(DataContext context) : Repo<DataContext, UserEntity>(context)
	{
		
	}
}
