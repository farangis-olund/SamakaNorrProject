using Infrastructure.Entities;
using Infrastructure.Factories;
using Infrastructure.Models;
using Infrastructure.Repositories;

namespace Infrastructure.Services;

public class NotificationService(NotificationRepository notificationRepository)
{
    private readonly NotificationRepository _notificationRepository = notificationRepository;

    public async Task<ResponseResult> AddNotificationAsync(NotificationEntity notification)
    {
        try
        {
            var newNotification = new NotificationEntity
            {
                UserId = notification.UserId,
                Title = notification.Title,
                Message = notification.Message,
                IsRead = false,
                CreatedAt = DateTime.UtcNow
            };

            return await _notificationRepository.AddAsync(newNotification);
        }
        catch (Exception ex)
        {
            return ResponseFactory.Error(ex.Message);
        }
    }

    public async Task<ResponseResult> GetNotificationAsync(int id)
    {
        try
        {
            return await _notificationRepository.GetOneAsync(c => c.Id == id);
        }
        catch (Exception ex)
        {
            return ResponseFactory.Error(ex.Message);
        }
    }

    public async Task<ResponseResult> GetAllNotificationsAsync(string userId)
    {
        try
        {
            var notifications = await _notificationRepository
                .GetAllAsync(c => c.UserId == userId);

            if (notifications != null && notifications.ContentResult is IEnumerable<NotificationEntity> list)
            {
                var ordered = list.OrderByDescending(n => n.CreatedAt).ToList();
                return ResponseFactory.Ok(ordered);
            }

            return ResponseFactory.NotFound();
        }
        catch (Exception ex)
        {
            return ResponseFactory.Error(ex.Message);
        }
    }


    public async Task<ResponseResult> GetUnreadCountAsync(string userId)
    {
        try
        {
            var response = await _notificationRepository.GetAllAsync(c => c.UserId == userId && !c.IsRead);

            if (response.StatusCode == StatusCode.Ok && response.ContentResult is IEnumerable<NotificationEntity> notifications)
            {
                var count = notifications.Count();
                return ResponseFactory.Ok(count);
            }

            return ResponseFactory.Ok(0); // no unread
        }
        catch (Exception ex)
        {
            return ResponseFactory.Error(ex.Message);
        }
    }

    public async Task<ResponseResult> MarkAsReadAsync(int id)
    {
        try
        {
            var response = await _notificationRepository.GetOneAsync(c => c.Id == id);

            if (response.StatusCode == StatusCode.Ok)
            {
                var existing = (NotificationEntity)response.ContentResult!;
                existing.IsRead = true;

                return await _notificationRepository.UpdateAsync(c => c.Id == id, existing);
            }

            return response;
        }
        catch (Exception ex)
        {
            return ResponseFactory.Error(ex.Message);
        }
    }

    public async Task<ResponseResult> DeleteNotificationAsync(int id)
    {
        try
        {
            var existing = await _notificationRepository.GetOneAsync(x => x.Id == id);

            if (existing != null)
            {
                await _notificationRepository.RemoveAsync(c => c.Id == id);
                return ResponseFactory.Ok("Notification removed successfully!");
            }

            return ResponseFactory.NotFound();
        }
        catch (Exception ex)
        {
            return ResponseFactory.Error(ex.Message);
        }
    }
}
