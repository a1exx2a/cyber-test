using Microsoft.AspNetCore.Mvc;

namespace CyberTest.API.Controllers;


[ApiController]
[Route("api/questions/{questionId:int}/[controller]")]
[ProducesResponseType(StatusCodes.Status500InternalServerError)]
public class AnswersController : ControllerBase
{
    private static readonly Dictionary<(int, int), AnswerData> _answers = new()
    {
        { (1, 1), new AnswerData(1, "Qwerty123", false, 1) },
        { (1, 2), new AnswerData(2, "Длинный пароль из 4+ слов", true, 1) },
        { (2, 3), new AnswerData(3, "Опечатки в адресе отправителя", true, 2) },
        { (2, 4), new AnswerData(4, "Срочность в тексте", true, 2) }
    };

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetAll([FromRoute] int questionId)
    {
        var result = _answers
            .Where(kv => kv.Key.Item1 == questionId)
            .Select(kv => kv.Value)
            .OrderBy(a => a.Id)
            .ToList();

        if (!result.Any())
            return NotFound($"Ответы для вопроса {questionId} не найдены.");

        return Ok(result);
    }

    [HttpGet("{answerId:int}")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetById([FromRoute] int questionId, [FromRoute] int answerId)
    {
        if (!_answers.TryGetValue((questionId, answerId), out var answer))
            return NotFound($"Ответ {answerId} для вопроса {questionId} не найден.");

        return Ok(answer);
    }

    [HttpPost]
    [ProducesResponseType(typeof(object), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    public IActionResult CreateAnswer([FromRoute] int questionId, [FromBody] CreateAnswerRequest request)
    {
        var NewId = 5;
        return Created($"/api/questions/{questionId}/answers/{NewId}",
            new AnswerData(NewId, request.Text, request.IsCorrect, questionId));
    }

    [HttpPut("{answerId:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult UpdateAnswer([FromRoute] int questionId, [FromRoute] int answerId, [FromBody] UpdateAnswerRequest request)
    {
        if (answerId != request.Id)
            return BadRequest("Идентификатор в пути и в теле не совпадают.");

        if (!_answers.ContainsKey((questionId, answerId)))
            return NotFound($"Ответ {answerId} для вопроса {questionId} не найден.");

        return NoContent();
    }

    [HttpDelete("{answerId:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult DeleteAnswer([FromRoute] int questionId, [FromRoute] int answerId)
    {
        if (!_answers.ContainsKey((questionId, answerId)))
            return NotFound($"Ответ {answerId} для вопроса {questionId} не найден.");

        return NoContent();
    }

    public record AnswerData(int Id, string Text, bool IsCorrect, int QuestionId);
    public record CreateAnswerRequest(string Text, bool IsCorrect);
    public record UpdateAnswerRequest(int Id, string Text, bool IsCorrect);
}