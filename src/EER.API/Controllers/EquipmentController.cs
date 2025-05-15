using EER.Domain.Entities;
using Microsoft.AspNetCore.Mvc;

namespace EER.API.Controllers;

[Route("api/[controller]")]
[ApiController]
public class EquipmentController : ControllerBase
{
    private static readonly Dictionary<long, Equipment> _equipment = [];
    private static long _idCounter;

    // GET: api/equipment
    [HttpGet]
    public IActionResult GetAll()
    {
        return Ok(_equipment.Values);
    }

    // GET: api/equipment/1
    [HttpGet("{id:long}")]
    public IActionResult GetById(long id)
    {
        return _equipment.TryGetValue(id, out var item) 
            ? Ok(item) 
            : NotFound();
    }

    // POST: api/equipment
    [HttpPost]
    public IActionResult Create(Equipment equipment)
    {
        equipment.Id = Interlocked.Increment(ref _idCounter);
        equipment.UpdatedAt = DateTime.UtcNow;
        _equipment[equipment.Id] = equipment;
        return CreatedAtAction(nameof(GetById), new { id = equipment.Id }, equipment);
    }

    // PUT: api/equipment/1
    [HttpPut("{id:long}")]
    public IActionResult Update(long id, Equipment updatedEquipment)
    {
        if (!_equipment.TryGetValue(id, out var equipment))
        {
            return NoContent();
        }

        equipment.Name = updatedEquipment.Name;
        equipment.Location = updatedEquipment.Location;
        equipment.CategoryId = updatedEquipment.CategoryId;
        equipment.AvailableQuantity = updatedEquipment.AvailableQuantity;
        equipment.Description = updatedEquipment.Description;
        equipment.PricePerDay = updatedEquipment.PricePerDay;
        equipment.UpdatedAt = DateTime.UtcNow;
        return Ok(equipment);
    }

    // DELETE: api/equipments/1
    [HttpDelete("{id:long}")]
    public IActionResult Delete(long id)
    {
        return !_equipment.Remove(id) 
            ? NotFound() 
            : NoContent();
    }
}