using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.TagHelpers;

namespace CyberTest.API.Controllers;

[ApiController]
[Route("api/[controller]")]
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
    public IActionResult GetAll()
    {
        return Ok(_groups.Values);
    }

    // Получить группу по id
    [HttpGet("{id:int}")]
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
    public IActionResult CreateGroup([FromBody] CreateGroupRequest request)
    {
        var new_id = 5;
        return Created($"/api/groups/{new_id}", new GroupData(new_id, request.Name, request.DirectionId, request.CourseId));
    }

    // Обновить уже существующую группу
    [HttpPut("{id:int}")]
    public IActionResult UpdateGroup([FromRoute] int id, [FromBody] UpdateGroupRequest request)
    {

        if (id != request.Id)
        {
            return BadRequest("Идентификатор в пути и в теле не совпадают.");
        }
     
        if (!_groups.ContainsKey(id))
        {
            return NotFound($"Группа с идентификатором {id} не найдена.");
        }

        return NoContent();
    }

    // Удаление существующей группы
    [HttpDelete("{id:int}")]
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
