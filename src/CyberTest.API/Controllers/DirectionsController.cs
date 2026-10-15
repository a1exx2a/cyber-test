using System.Text.RegularExpressions;

using Microsoft.AspNetCore.Mvc;

namespace CyberTest.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[ProducesResponseType(StatusCodes.Status500InternalServerError)]
public class DirectionsController : ControllerBase
{
    private static readonly Dictionary<int, DirectionData> _directions = new()
    {
        { 1, new DirectionData(1, "Backend-разработка") },
        { 2, new DirectionData(2, "Frontend-разработка") },
        { 3, new DirectionData(3, "Мобильная разработка") },
        { 4, new DirectionData(4, "Кибербезопасность") },
        { 5, new DirectionData(5, "Data Science") }
    };

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<object>), StatusCodes.Status200OK)]
    public IActionResult GetAll()
    {
        return Ok(_directions.Values.OrderBy(d => d.Id));
    }


    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetById([FromRoute] int id)
    {
        if (!_directions.TryGetValue(id, out var direction))
            return NotFound($"Направление с идентификатором {id} не найдено.");
        return Ok(direction);
    }

    [HttpPost]
    [ProducesResponseType(typeof(object), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    public IActionResult CreateDirection([FromBody] CreatedDirectionRequest request)
    {
        var NewId = 6;
        return Created($"/api/directions/{NewId}", new DirectionData(NewId, request.Name));
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult UpdateDirection([FromRoute] int id, [FromBody] UpdateDirectionRequest request)
    {
        if (id != request.Id) return BadRequest("Идентификатор в пути и в теле не совпадают.");

        if (!_directions.ContainsKey(id)) return NotFound($"Направление с идентификатором {id} не найдена.");

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult DeleteDirection([FromRoute] int id)
    {
        if (!_directions.ContainsKey(id)) return NotFound($"Направление с идентификатором {id} не найдено.");
        return NoContent();
    }

    public record DirectionData(int Id, string Name);
    public record CreatedDirectionRequest(string Name);
    public record UpdateDirectionRequest(int Id, string Name);
}