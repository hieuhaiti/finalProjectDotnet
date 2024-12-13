using Microsoft.AspNetCore.Mvc;
using Server.Models;
using Server.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

[ApiController]
[Route("api/[controller]")]
public class CoordinatesController : ControllerBase
{
    private readonly CoordinatesService _service;

    public CoordinatesController(CoordinatesService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<IActionResult> GetAllDistricts()
    {
        var districts = await _service.GetAllDistrictsAsync();
        return Ok(districts);
    }

    [HttpPost]
    public async Task<IActionResult> AddDistrict([FromBody] Coordinate request)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var createdDistrict = await _service.AddDistrictAsync(
            request.lon,
            request.lat,
            request.district,
            request.description);

        return CreatedAtAction(nameof(GetAllDistricts),
            new { idDistrict = createdDistrict.id },
            createdDistrict);
    }


    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateDistrict(int id, Coordinate updatedDistrict)
    {
        var district = await _service.UpdateDistrictAsync(id, updatedDistrict);
        if (district == null) return NotFound();
        return Ok(district);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteDistrict(Guid id)
    {
        var success = await _service.DeleteDistrictAsync(id);
        if (!success) return NotFound();
        return NoContent();
    }
}