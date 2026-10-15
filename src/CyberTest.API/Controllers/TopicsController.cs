using System.Text.RegularExpressions;

using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace CyberTest.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[ProducesResponseType(StatusCodes.Status500InternalServerError)]
public class TopicsController : ControllerBase
{
    private static readonly Dictionary<int, string> _topics = new()
    {
        { 1, "Парольная безопасность" },
        { 2, "Фишинг и социальная инженерия" },
        { 3, "Безопасность в социальных сетях" },
        { 4, "Защита персональных данных" },
        { 5, "Безопасность мобильных устройств" },
        { 6, "Wi-Fi безопасность" },
        { 7, "Основы криптографии" },
        { 8, "Цифровая гигиена" },
        { 9, "Законодательство в сфере кибербезопасности" }
    };

    // Получить список всех тем кибербезопасности с пагинацией
    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<object>), StatusCodes.Status200OK)]
    public IActionResult GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
    {
        // Задаём минимальные и максимальные параметры 
        if (page < 1) page = 1;
        if (pageSize < 1) pageSize = 10;
        if (pageSize > 100) pageSize = 100;

        var totalCount = _topics.Count;
        var totalPages = (int)Math.Ceiling(totalCount / (double)pageSize);
        var offset = (page - 1) * pageSize;

        var items = _topics
            .OrderBy(t => t.Key)
            .Skip(offset)
            .Take(pageSize)
            .Select(t => new { Id = t.Key, Name = t.Value })
            .ToList();

        var response = new
        {
            Page = page,
            PageSize = pageSize,
            TotalCount = totalCount,
            TotalPages = totalPages,
            Items = items
        };

        return Ok(response);
    }

    // Поиск темы по id
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetById([FromRoute] int id)
    {

        if (!_topics.TryGetValue(id, out var name))
        {
            return NotFound($"Тема с идентификатором {id} не найдена.");
        }

        return Ok(new {Id = id, Name = name});
    }

    [HttpGet("search")]
    [ProducesResponseType(typeof(IEnumerable<object>), StatusCodes.Status200OK)]
    public IActionResult Search([FromQuery] string? q)
    {
        // Если SearchParametr не передан или пустой - вернуть весь список
        if (string.IsNullOrWhiteSpace(q))
        {
            var AllData = _topics
                .OrderBy(t => t.Key)
                .Select(t => new {Id = t.Key, Name = t.Value});

            return Ok(AllData);
        }

        // Иначе - фильтруем по подстрочке без учета регистра
        var FilteredData = _topics
            .Where(t => t.Value.Contains(q, StringComparison.OrdinalIgnoreCase))
            .OrderBy(t => t.Key)
            .Select(t => new { Id =  t.Key, Name = t.Value});

        return Ok(FilteredData);
    }

    // Создание темы
    [HttpPost]
    [ProducesResponseType(typeof(object), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    public IActionResult CreateTopic([FromBody] CreateTopicRequest request)
    {
        var NewId = 10;
        return Created($"/api/topics/{NewId}", new {Id = NewId, Name = request.Name});
    }


    // Обновить уже существующую тему
    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult UpdateTopic([FromRoute] int id, [FromBody] UpdateTopicRequest request)
    {
        if (id != request.Id)
        {
            return BadRequest("Идентификатор в пути и в теле не совпадают.");
        }

        
        if (!_topics.ContainsKey(id))
        {
            return NotFound($"Тема с идентификатором {id} не найдена.");
        }

        return NoContent(); // получаем 204 успех без тела ответа
    }


    // Удаление существующей темы
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult DeleteTopic([FromRoute] int id)
    {
        
        if (!_topics.ContainsKey(id))
        {
            return NotFound($"Тема с идентификатором {id} не найдена.");
        }

        return NoContent();  // получаем 204 успех без тела ответа
    }

    // Модель запроса CreateTopic
    public record CreateTopicRequest(string Name, string Slug);
    // Модель запроса UpdateTopic
    public record UpdateTopicRequest(int Id, string Name, string Slug);

}