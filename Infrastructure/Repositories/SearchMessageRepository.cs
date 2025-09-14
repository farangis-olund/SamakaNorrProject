
using Infrastructure.Contexts;
using Infrastructure.Entities;
using Infrastructure.Factories;
using Infrastructure.Models;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Infrastructure.Repositories
{
    public class SearchMessageRepository(DataContext context) : Repo<DataContext, SearchMessageEntity>(context)
        {
            public override async Task<ResponseResult> GetAllAsync(Expression<Func<SearchMessageEntity, bool>> predicate)
            {
                try
                {
                    var messages = await _context.SearchMessages
                        .Include(m => m.User)              
                        .Include(m => m.SearchRequest)         // ✅ include request if needed
                        .Where(predicate)
                        .OrderBy(m => m.Timestamp)             // oldest first
                        .ToListAsync();

                    return ResponseFactory.Ok(messages);
                }
                catch (Exception ex)
                {
                    return ResponseFactory.Error("Error fetching messages: " + ex.Message);
                }
            }

            public override async Task<ResponseResult> GetOneAsync(Expression<Func<SearchMessageEntity, bool>> predicate)
            {
                try
                {
                    var message = await _context.SearchMessages
                        .Include(m => m.User)
                        .Include(m => m.SearchRequest)
                        .FirstOrDefaultAsync(predicate);

                    if (message == null)
                        return ResponseFactory.NotFound("Message not found.");

                    return ResponseFactory.Ok(message);
                }
                catch (Exception ex)
                {
                    return ResponseFactory.Error("Error fetching message: " + ex.Message);
                }
            }
        }
   }


