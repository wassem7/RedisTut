using System.Net;
using Caching.Data;
using Caching.Models;
using Caching.Models.Dtos;
using Caching.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Caching.Controllers;

[ApiController]
[Route("api/drivers")]
public class DriverController
{
    private readonly ApplicationDbContext _db;
    private readonly ICacheService _cacheService;
    private ApiResponse _apiResponse;

    public DriverController(ApplicationDbContext db, ICacheService cacheService)
    {
        _db = db;
        _cacheService = cacheService;
        _apiResponse = new ApiResponse();
    }

    [HttpGet("Getalldrivers")]
    public async Task<ApiResponse> GetAllDrivers()
    {
        // check if cache has data


        var cacheData = _cacheService.GetData<IEnumerable<Driver>>("drivers");
        if (cacheData is not null && cacheData.Count() != 0)
        {
            _apiResponse.Result = cacheData;
            _apiResponse.IsSuccess = true;
            _apiResponse.SuccessMessage = "Cached data retrieved";
            _apiResponse.HttpStatusCode = HttpStatusCode.OK;
            return _apiResponse;
        }

        cacheData = await _db.Drivers.ToListAsync();
        var expiryTime = DateTimeOffset.Now.AddMinutes(3);
        _cacheService.SetData("drivers", cacheData, expiryTime);

        _apiResponse.Result = cacheData;
        _apiResponse.IsSuccess = true;
        _apiResponse.SuccessMessage = "DB data retrieved";
        _apiResponse.HttpStatusCode = HttpStatusCode.OK;
        return _apiResponse;
    }

    [HttpPost("AddDriver")]
    public async Task<ApiResponse> AddDriver([FromBody] AddDriverDto addDriverDto)
    {
        var driverDto = new Driver()
        {
            DriverNumber = addDriverDto.DriverNumber,
            Name = addDriverDto.Name
        };
        var driver = await _db.Drivers.AddAsync(driverDto);

        await _db.SaveChangesAsync();

        var expiryTime = DateTimeOffset.Now.AddMinutes(30);
        _cacheService.SetData($"$driver:{driver.Entity.Id}", driver.Entity, expiryTime);
        _apiResponse.Result = driver.Entity;
        _apiResponse.IsSuccess = true;
        _apiResponse.SuccessMessage = "Driver added !";
        _apiResponse.HttpStatusCode = HttpStatusCode.OK;
        return _apiResponse;
    }

    [HttpDelete("{id:guid}")]
    public async Task<ApiResponse> DeleteDriver(Guid id)
    {
        var driver = await _db.Drivers.FindAsync(id);

        if (driver is null)
        {
            _apiResponse.IsSuccess = true;
            _apiResponse.SuccessMessage = "Driver not found";
            _apiResponse.HttpStatusCode = HttpStatusCode.NotFound;
            return _apiResponse;
        }

        _db.Remove(driver);
        await _db.SaveChangesAsync();
        var expiryTime = DateTimeOffset.Now.AddMinutes(30);
        var drivers = await _db.Drivers.ToListAsync();
        _cacheService.RemoveData("drivers");
        _cacheService.SetData("drivers", drivers, expiryTime);

        _apiResponse.IsSuccess = true;
        _apiResponse.SuccessMessage = "Driver deleted !";
        _apiResponse.HttpStatusCode = HttpStatusCode.OK;
        return _apiResponse;
    }
}
