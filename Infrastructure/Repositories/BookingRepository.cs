
using Infrastructure.Contexts;
using Infrastructure.Entities;
using Infrastructure.Factories;
using Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Infrastructure.Repositories
{
    public class BookingRepository(DataContext context) : Repo<DataContext, BookingEntity>(context)
    {
        public override async Task<ResponseResult> GetAllAsync(Expression<Func<BookingEntity, bool>> predicate)
        {
            try
            {
                var bookings = await _context.Bookings
                    .Include(b => b.Ride) 
                    .Where(predicate)
                    .ToListAsync();

                return ResponseFactory.Ok(bookings);
            }
            catch (Exception ex)
            {
                return ResponseFactory.Error("Failed to get bookings: " + ex.Message);
            }
        }
    }


}