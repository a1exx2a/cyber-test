using Microsoft.AspNetCore.Mvc;

namespace CyberTest.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[ProducesResponseType(StatusCodes.Status500InternalServerError)]
public class ProjectsController : ControllerBase
{
    private static readonly Dictionary<int, ProjectData> _projects = new()
    {
        { 1, new ProjectData(1, "КОД") },
        { 2, new ProjectData(2, "ПАЗЛ") }
    };

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<object>), StatusCodes.Status200OK)]
    public IActionResult GetAll()
    {
        return Ok(_projects.Values.OrderBy(p => p.Id));
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetById([FromRoute] int id)
    {
        if (!_projects.TryGetValue(id, out var project)) return NotFound($"Проект с идентификатором {id} не найден.");

        return Ok(project);
    }

    [HttpPost]
    [ProducesResponseType(typeof(object), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    public IActionResult CreateCourse([FromBody] CreateProjectRequest request)
    {
        var NewId = 3;
        return Created($"/api/projects/{NewId}", new ProjectData(NewId, request.Name));
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult UpdateCourse([FromRoute] int id, [FromBody] UpdateProjectRequest request)
    {
        if (id != request.Id) return BadRequest("Идентификатор в пути и в теле не совпадают.");

        if (!_projects.ContainsKey(id)) return NotFound($"Проект с идентификатором {id} не найден.");
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult DeleteCourse([FromRoute] int id)
    {
        if (!_projects.ContainsKey(id)) return NotFound($"Проект с идентификатором {id} не найден.");
        return NoContent();
    }

    public record ProjectData(int Id, string Name);
    public record CreateProjectRequest(string Name);
    public record UpdateProjectRequest(int Id, string Name);
}