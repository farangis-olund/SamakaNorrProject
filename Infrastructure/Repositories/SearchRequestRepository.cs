using Infrastructure.Contexts;
using Infrastructure.Entities;
using Infrastructure.Factories;
using Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Infrastructure.Repositories
{
    public class SearchRequestRepository(DataContext context) : Repo<DataContext, SearchRequestEntity>(context)
    {
        public override async Task<ResponseResult> GetAllAsync(Expression<Func<SearchRequestEntity, bool>> predicate)
        {
            try
            {
                var rides = await _context.SearchRequests
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

        public override async Task<ResponseResult> GetAllAsync()
        {
            try
            {
                var rides = await _context.SearchRequests
                    .Include(m => m.Messages)
                     .ToListAsync();

                return ResponseFactory.Ok(rides);
            }
            catch (Exception ex)
            {
                return ResponseFactory.Error("Error fetching rides with bookings: " + ex.Message);
            }
        }

        public override async Task<ResponseResult> GetOneAsync(Expression<Func<SearchRequestEntity, bool>> predicate)
        {
            try
            {
                var ride = await _context.SearchRequests
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


