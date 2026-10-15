using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.TagHelpers;

namespace CyberTest.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[ProducesResponseType(StatusCodes.Status500InternalServerError)]
public class GroupsController : ControllerBase
{
    private static readonly Dictionary<int, GroupData> _groups = new()
    {
        { 1, new GroupData(1, "КБ-101", 1, 1) },
        { 2, new GroupData(2, "КБ-102", 1, 1) },
        { 3, new GroupData(3, "ИБ-201", 2, 2) },
        { 4, new GroupData(4, "ИБ-202", 2, 2)}
    };


    // Получить список всех групп 
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<object>), StatusCodes.Status200OK)]
    public IActionResult GetAll()
    {
        return Ok(_groups.Values);
    }

    // Получить группу по id
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetById([FromRoute] int id)
    {

        if (!_groups.TryGetValue(id, out var group_Data))
        {
            return NotFound($"Группа с идентификатором {id} не найдена.");
        }

        return Ok(group_Data);
    }

    // Создание группы
    [HttpPost]
    [ProducesResponseType(typeof(object), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    public IActionResult CreateGroup([FromBody] CreateGroupRequest request)
    {
        var NewId = 5;
        return Created($"/api/groups/{NewId}", new GroupData(NewId, request.Name, request.DirectionId, request.CourseId));
    }

    // Обновить уже существующую группу
    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult UpdateGroup([FromRoute] int id, [FromBody] UpdateGroupRequest request)
    {

        if (id != request.Id) return BadRequest("Идентификатор в пути и в теле не совпадают.");
     
        if (!_groups.ContainsKey(id)) return NotFound($"Группа с идентификатором {id} не найдена.");

        return NoContent();
    }

    // Удаление существующей группы

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult DeleteGroup([FromRoute] int id)
    {

        if (!_groups.ContainsKey(id))
        {
            return NotFound($"Группа с идентификатором {id} не найдена.");
        }

        return NoContent();
    }


    public record GroupData(int Id, string Name, int DirectionId, int CourseId);
    public record CreateGroupRequest(string Name, int DirectionId, int CourseId);
    public record UpdateGroupRequest(int Id, string Name, int DirectionId, int CourseId);
}
