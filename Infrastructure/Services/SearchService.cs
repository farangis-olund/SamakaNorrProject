using Infrastructure.Entities;
using Infrastructure.Factories;
using Infrastructure.Models;
using Infrastructure.Repositories;
using Microsoft.AspNetCore.Identity;


namespace Infrastructure.Services
{
    public class SearchService(SearchRequestRepository searchRepository, UserManager<UserEntity> userManager)
    {
        private readonly SearchRequestRepository _searchRepository = searchRepository;
        private readonly UserManager<UserEntity> _userManager = userManager;

        public async Task<ResponseResult> AddRequestAsync(SearchRequestModel model, string userEmail)
        {
            try
            {
                var userEntity = await _userManager.FindByEmailAsync(userEmail);
                if (userEntity == null)
                    return ResponseFactory.NotFound("User not found, please register first.");

                var newRequest = new SearchRequestEntity
                {
                    UserId = userEntity.Id,
                    Origin = model.Origin,
                    Destination = model.Destination,
                    DepartureTime = model.DepartureTime!.Value,
                    SeatsRequired = model.SeatsRequired,
                    Notes = model.Notes,
                    CreatedAt = DateTime.Now
                };

                var result = await _searchRepository.AddAsync(newRequest);
                return ResponseFactory.Ok(result);
            }
            catch (Exception ex)
            {
                return ResponseFactory.Error(ex.Message);
            }
        }

        public async Task<ResponseResult> GetRequestAsync(int id)
        {
            try
            {
                var result = await _searchRepository.GetOneAsync(r => r.Id == id);
                if (result.StatusCode == StatusCode.Ok)
                    return ResponseFactory.Ok(result.ContentResult!);
                return ResponseFactory.NotFound();
            }
            catch (Exception ex)
            {
                return ResponseFactory.Error(ex.Message);
            }
        }

        public async Task<ResponseResult> GetAllRequestsAsync()
        {
            try
            {
                var result = await _searchRepository.GetAllAsync();
                if (result.StatusCode == StatusCode.Ok)
                    return ResponseFactory.Ok(result.ContentResult!);
                return ResponseFactory.NotFound("No requests found.");
            }
            catch (Exception ex)
            {
                return ResponseFactory.Error(ex.Message);
            }
        }

        public async Task<ResponseResult> GetAllRequestsAsync(string userId)
        {
            try
            {
                var result = await _searchRepository.GetAllAsync(r => r.UserId == userId);
                if (result.StatusCode == StatusCode.Ok)
                    return ResponseFactory.Ok(result.ContentResult!);
                return ResponseFactory.NotFound("No requests for this user.");
            }
            catch (Exception ex)
            {
                return ResponseFactory.Error(ex.Message);
            }
        }

        public async Task<ResponseResult> DeleteRequestAsync(int id)
        {
            try
            {
                var existing = await _searchRepository.GetOneAsync(x => x.Id == id);

                if (existing.StatusCode == StatusCode.Ok)
                {
                    await _searchRepository.RemoveAsync(r => r.Id == id);
                    return ResponseFactory.Ok("Successfully removed!");
                }

                return ResponseFactory.NotFound();
            }
            catch (Exception ex)
            {
                return ResponseFactory.Error(ex.Message);
            }
        }

        public async Task<ResponseResult> SearchRequestsAsync(RideSearchModel model)
        {
            try
            {
                var allRequestsResponse = await GetAllRequestsAsync();

                if (allRequestsResponse.StatusCode == StatusCode.Ok)
                {
                    var allRequests = (List<SearchRequestEntity>)allRequestsResponse.ContentResult!;

                    var filtered = allRequests.Where(r =>
                        (string.IsNullOrEmpty(model.Origin) || r.Origin.Contains(model.Origin, StringComparison.OrdinalIgnoreCase)) &&
                        (string.IsNullOrEmpty(model.Destination) || r.Destination.Contains(model.Destination, StringComparison.OrdinalIgnoreCase)) &&
                        (!model.DepartureTime.HasValue || r.DepartureTime.Date == model.DepartureTime.Value.Date)
                    ).ToList();

                    return ResponseFactory.Ok(filtered);
                }

                return ResponseFactory.NotFound("No requests found.");
            }
            catch (Exception ex)
            {
                return ResponseFactory.Error(ex.Message);
            }
        }
    }
}
