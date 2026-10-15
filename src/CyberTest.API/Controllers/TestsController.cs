using Microsoft.AspNetCore.Mvc;

namespace CyberTest.API.Controllers;


[ApiController]
[Route("api/[controller]")]
[ProducesResponseType(StatusCodes.Status500InternalServerError)]
public class TestsController : ControllerBase
{
    private static readonly Dictionary<int, TestData> _tests = new()
    {
        { 1, new TestData(1, "Основы парольной безопасности", 1, false) },
        { 2, new TestData(2, "Как распознать фишинг",            2, true) },
        { 3, new TestData(3, "Безопасность в социальных сетях", 3, false) }
    };

    // Фильтрация по теме (topicId) и публичности (isPublic).
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<object>), StatusCodes.Status200OK)]
    public IActionResult GetAll([FromQuery] int? topicId = null, [FromQuery] bool? isPublic = null)
    {
        var query = _tests.Values.AsQueryable();

        // Если клиент передал topicId — фильтруем.
        if (topicId.HasValue) query = query.Where(t => t.TopicId == topicId.Value);

        // Если клиент передал isPublic — фильтруем по нему.
        if (isPublic.HasValue) query = query.Where(t => t.IsPublic == isPublic.Value);
        return Ok(query.OrderBy(t => t.Id));
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetById([FromRoute] int id)
    {
        if (!_tests.TryGetValue(id, out var test)) return NotFound($"Тест с идентификатором {id} не найден.");
        return Ok(test);
    }

    [HttpPost]
    [ProducesResponseType(typeof(object), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    public IActionResult CreateTest([FromBody] CreateTestRequest request)
    {
        var NewId = 4;
        return Created($"/api/tests/{NewId}", new TestData(NewId, request.Title, request.TopicId, false));
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult UpdateTest([FromRoute] int id, [FromBody] UpdateTestRequest request)
    {
        if (id != request.Id) return BadRequest("Идентификатор в пути и в теле не совпадают.");

        if (!_tests.ContainsKey(id)) return NotFound($"Тест с идентификатором {id} не найден.");
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult DeleteTest([FromRoute] int id)
    {
        if (!_tests.ContainsKey(id)) return NotFound($"Тест с идентификатором {id} не найден.");
        return NoContent();
    }

    public record TestData(int Id, string Title, int TopicId, bool IsPublic);
    public record CreateTestRequest(string Title, int TopicId);
    public record UpdateTestRequest(int Id, string Title, int TopicId);
}