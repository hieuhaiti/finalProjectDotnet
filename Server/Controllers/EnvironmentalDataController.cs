using Microsoft.AspNetCore.Mvc;
using Server.Models;
using Server.Services;

[ApiController]
[Route("")]
public class Introduction : ControllerBase
{
    [HttpGet]
    public IActionResult Get()
    {
        return
        Ok("finnal project dotnet \nApi by Ha Trung Hieu - 2121051127 - HUMG \ntry: /swagger");
    }
}

[Route("api/[controller]")]
public class EnvironmentalDataController : ControllerBase
{
    private readonly EnvironmentalDataService _service;

    public EnvironmentalDataController(EnvironmentalDataService service)
    {
        _service = service;
    }

    [HttpGet()]
    public async Task<IActionResult> GetCurrentData()
    {
        var data = await _service.GetCurrentDataAsync();
        return Ok(data);
    }

    [HttpGet("all")]
    public async Task<IActionResult> GetAll()
    {
        var data = await _service.GetAllAsync();
        return Ok(data);
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id)
    {
        var data = await _service.GetByIdAsync(id);
        if (data == null) return NotFound();
        return Ok(data);
    }

    [HttpGet("district/id/{districtId}")]
    public async Task<IActionResult> GetAllByDistrict(int districtId)
    {
        var data = await _service.GetAllByDistrictIdAsync(districtId);
        return Ok(data);
    }

    [HttpGet("district/name/{districtName}")]
    public async Task<IActionResult> GetAllByDistrict(string districtName)
    {
        var data = await _service.GetAllByDistrictNameAsync(districtName);
        return Ok(data);
    }

    [HttpGet("district/id/{districtId}/start={startDate}&end={endDate}")]
    public async Task<IActionResult> GetAllByDistrictIdAndDate(int districtId, long startDate, long endDate)
    {
        var data = await _service.GetAllByDistrictIdAndDate(districtId, startDate, endDate);
        return Ok(data);
    }

    [HttpGet("district/name/{districtName}/start={startDate}&end={endDate}")]
    public async Task<IActionResult> GetAllByDistrictNameAndDate(string districtName, long startDate, long endDate)
    {
        var data = await _service.GetAllByDistrictNameAndDate(districtName, startDate, endDate);
        return Ok(data);
    }

    [HttpGet("date/{date}")]
    public async Task<IActionResult> GetAllByDate(long date)
    {
        var data = await _service.GetAllByDateAsync(date);
        return Ok(data);
    }

    [HttpGet("start={startDate}&end={endDate}")]
    public async Task<IActionResult> GetByDateRange(long startDate, long endDate)
    {
        var data = await _service.GetByDateRangeAsync(startDate, endDate);
        return Ok(data);
    }

    [HttpPost("current")]
    public async Task<IActionResult> AddCurrentData(EnvironmentalDataEntry entry)
    {
        var createdEntry = await _service.AddCurrentDataAsync(entry);
        return CreatedAtAction(nameof(GetById), new { Id = createdEntry.id }, createdEntry);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, EnvironmentalDataEntry updatedEntry)
    {
        var entry = await _service.UpdateAsync(id, updatedEntry);
        if (entry == null) return NotFound();
        return Ok(entry);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var success = await _service.DeleteAsync(id);
        if (!success) return NotFound();
        return NoContent();
    }
}