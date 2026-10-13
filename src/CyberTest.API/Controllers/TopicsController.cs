using System.Text.RegularExpressions;

using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace CyberTest.API.Controllers;

[ApiController]
[Route("api/[controller]")]
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

    // Получить список всех тем кибербезопасности
    [HttpGet]
    public IActionResult GetAll()
    {
        var result = _topics.Select(t => new {Id = t.Key, Name = t.Value});
        return Ok(result);
    }

    // Поиск темы по id
    [HttpGet("{id:int}")]
    public IActionResult GetById([FromRoute] int id)
    {

        if (!_topics.TryGetValue(id, out var name))
        {
            return NotFound($"Тема с идентификатором {id} не найдена.");
        }

        return Ok(new {Id = id, Name = name});
    }

    // Создание темы
    [HttpPost]
    public IActionResult CreateTopic([FromBody] CreateTopicRequest request)
    {
        var new_id = 10;
        return Created($"/api/topics/{new_id}", new {Id = new_id, Name = request.Name});
    }


    // Обновить уже существующую тему
    [HttpPut("{id:int}")]
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