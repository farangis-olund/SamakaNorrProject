
using Infrastructure.Contexts;
using Infrastructure.Entities;
using Infrastructure.Factories;
using Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Infrastructure.Repositories
{
    public class RideRepository(DataContext context) : Repo<DataContext, RideEntity>(context)
    {
        public override async Task<ResponseResult> GetAllAsync(Expression<Func<RideEntity, bool>> predicate)
        {
            try
            {
                var rides = await _context.Rides
                    .Include(r => r.Bookings)
					.Include(m => m.Messages)
					.Where(predicate)
                    .ToListAsync();

                return ResponseFactory.Ok(rides);
            }
            catch (Exception ex)
            {
                return ResponseFactory.Error("Error fetching rides with bookings: " + ex.Message);
            }
        }

		public override async Task<ResponseResult> GetOneAsync(Expression<Func<RideEntity, bool>> predicate)
		{
			try
			{
				var ride = await _context.Rides
					.Include(r => r.Bookings)
					.Include(r => r.Messages)
					.FirstOrDefaultAsync(predicate); 

				if (ride == null)
					return ResponseFactory.NotFound("Ride not found.");

				return ResponseFactory.Ok(ride); 
			}
			catch (Exception ex)
			{
				return ResponseFactory.Error("Error fetching ride: " + ex.Message);
			}
		}

	}
}