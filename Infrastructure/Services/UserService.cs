using Infrastructure.Entities;
using Infrastructure.Factories;
using Infrastructure.Models;
using Infrastructure.Repositories;

namespace Infrastructure.Services;

public class UserService(UserRepository userRepository)
{
	private readonly UserRepository _userRepository = userRepository;

	public async Task<ResponseResult> SingUpAsync(SignUpModel user)
	{
		try
		{
			var response = await _userRepository.GetOneAsync(c => c.Email == user.Email);

			if (response != null)
			{
				return ResponseFactory.Exists();
			}
			var newuser = new UserEntity
			{
				Id = Guid.NewGuid().ToString(),
				FirstName = user.FirstName,
				LastName = user.LastName,
				Email = user.Email
			};
			//(newuser.SecurityKey, newuser.Password) = PasswordHasherService.GenerateSecurePassword(user.Password);

			//await _userRepository.AddAsync(newuser);
			return ResponseFactory.Ok("Successfully signed up!");
		}
		catch (Exception ex)
		{
			return ResponseFactory.Error(ex.Message);
		}
	}

	public async Task<ResponseResult>SignInAsync(SignInModel model)
	{
		try
		{
			var response = await _userRepository.GetOneAsync(c => c.Email == model.Email);
			UserEntity user = (UserEntity)response.ContentResult!;
			if (response != null)
			{
				//if (PasswordHasherService.ValidateSecurePassword(model.Password, user.SecurityKey, user.Password))
				//{
				//	var newUser = new User
				//	{
				//		Id = user.Id,
				//		FirstName = user.FirstName,
				//		LastName = user.LastName,
				//		Email = user.Email

				//	};
				//	return ResponseFactory.Ok(newUser);
				//}
				return ResponseFactory.NotFound("User authantication failed!");
			}
			return ResponseFactory.NotFound();

		}
		catch (Exception ex)
		{
			return ResponseFactory.Error(ex.Message);
		}
	}

	public async Task<ResponseResult> GetAllUsersAsync()
	{
		try
		{
			var userEntities = await _userRepository.GetAllAsync();

			if (userEntities != null)
				return userEntities;
			return ResponseFactory.NotFound();
		}
		catch (Exception ex)
		{
			return ResponseFactory.Error(ex.Message);
		}
	}

	public async Task<ResponseResult> UpdateUserAsync(UserEntity user)
	{
		try
		{
			var response = await _userRepository.GetOneAsync(c => c.Id == user.Id);

			if (response.StatusCode == StatusCode.Ok)
			{
				var existinguser = (UserEntity)response.ContentResult!;
				

				var updateResponse = await _userRepository.UpdateAsync(c => c.Id == user.Id, existinguser);

				return updateResponse;
			}
			else
			{
				return response;
			}
		}
		catch (Exception ex)
		{
			return ResponseFactory.Error(ex.Message);
		}
	}

	public async Task<ResponseResult> DeleteUserAsync(string id)
	{
		try
		{
			var existinguser = await _userRepository.GetOneAsync(x => x.Id == id);

			if (existinguser != null)
			{
				await _userRepository.RemoveAsync(c => c.Id == id);
				return ResponseFactory.Ok("Successfully removed!");
			}

			return ResponseFactory.NotFound();
		}
		catch (Exception ex)
		{
			return ResponseFactory.Error(ex.Message);
		}
	}
}
