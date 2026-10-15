using Microsoft.AspNetCore.Mvc;

namespace CyberTest.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[ProducesResponseType(StatusCodes.Status500InternalServerError)]
public class CourseController : ControllerBase
{
    private static readonly Dictionary<int, CourseData> _course = new()
    {
        { 1, new CourseData(1, "1 курс") },
        { 2, new CourseData(2, "2 курс") },
        { 3, new CourseData(3, "3 курс") },
        { 4, new CourseData(4, "4 курс") }
    };

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<object>), StatusCodes.Status200OK)]
    public IActionResult GetAll()
    {
        return Ok(_course.Values.OrderBy(c => c.Id));
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetById([FromRoute] int id)
    {
        if (!_course.TryGetValue(id, out var course)) return NotFound($"Курс с идентификатором {id} не найден.");

        return Ok(course);
    }

    [HttpPost]
    [ProducesResponseType(typeof(object), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    public IActionResult CreateCourse([FromRoute] CourseData request)
    {
        var NewId = 5;
        return Created($"/api/course/{NewId}", new CourseData(NewId, request.Name));
    }


    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult UpdateCourse([FromRoute] int id, [FromBody] UpdateCourseRequest request)
    {
        if (id != request.Id) return BadRequest("Идентификатор в пути и в теле не совпадают.");

        if (!_course.ContainsKey(id)) return NotFound($"Курс с идентификатором {id} не найден.");

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult DeleteCourse([FromRoute] int id)
    {
        if (!_course.ContainsKey(id)) return NotFound($"Курс с идентификатор {id} не найден.");
        return NoContent();
    }



    public record CourseData(int Id, string Name);
    public record CreateCourseRequest(string Name);
    public record UpdateCourseRequest(int Id, string Name);
}