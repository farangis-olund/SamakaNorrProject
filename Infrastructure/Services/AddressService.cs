
using Infrastructure.Entities;
using Infrastructure.Factories;
using Infrastructure.Models;
using Infrastructure.Repositories;

namespace Infrastructure.Services;

public class AddressService(AddressRepository addressRepository)
{
    private readonly AddressRepository _addressRepository = addressRepository;

	public async Task<ResponseResult> AddAddressAsync(AddressEntity address)
    {
        try
        {
            var existingAddress = await _addressRepository.GetOneAsync(c => c.Id == address.Id);

            if (existingAddress != null)
            {
                return null!;
            }
            var newAddress = new AddressEntity
            {
                Addressline_1 = address.Addressline_1,
                Addressline_2 = address.Addressline_2,
                City = address.City,
                PostalCode = address.PostalCode

            };

            return await _addressRepository.AddAsync(newAddress);
        }
        catch (Exception ex)
        {
            return ResponseFactory.Error(ex.Message);
        }
    }

    public async Task<ResponseResult> GetAddressAsync(int id)
    {
        try
        {
            return await _addressRepository.GetOneAsync(c => c.Id == id);

        }
        catch (Exception ex)
        {
            return ResponseFactory.Error(ex.Message);
        }
    }

    public async Task<ResponseResult> GetAllAddresssAsync()
    {
        try
        {
            var AddressEntities = await _addressRepository.GetAllAsync();

            if (AddressEntities != null)
                return AddressEntities;
            return ResponseFactory.NotFound();
        }
        catch (Exception ex)
        {
            return ResponseFactory.Error(ex.Message);
        }
    }

    public async Task<ResponseResult> UpdateAddressAsync(AddressEntity address)
    {
        try
        {
            var response = await _addressRepository.GetOneAsync(c => c.Id == address.Id);

            if (response.StatusCode == StatusCode.Ok)
            {
                var existingAddress = (AddressEntity)response.ContentResult!;
                existingAddress.Addressline_1 = address.Addressline_1;
                existingAddress.Addressline_2 = address.Addressline_2;
                existingAddress.City = address.City;
                existingAddress.PostalCode = address.PostalCode;



                var updateResponse = await _addressRepository.UpdateAsync(c => c.Id == address.Id, existingAddress);

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

    public async Task<ResponseResult> DeleteAddressAsync(int id)
    {
        try
        {
            var existingAddress = await _addressRepository.GetOneAsync(x => x.Id == id);

            if (existingAddress != null)
            {
                await _addressRepository.RemoveAsync(c => c.Id == id);
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
