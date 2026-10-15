using Microsoft.AspNetCore.Mvc;

namespace CyberTest.API.Controllers;

[ApiController]
[Route("api/tests/{testId:int}/[controller]")]
[ProducesResponseType(StatusCodes.Status500InternalServerError)]
public class QuestionsController : ControllerBase
{
    // Ключ — tuple (testId, questionId).
    // Мы не можем использовать просто questionId, потому что
    // вопрос с id=1 в тесте 1 и вопрос с id=1 в тесте 2 — разные.
    private static readonly Dictionary<(int, int), QuestionData> _questions = new()
    {
        { (1, 1), new QuestionData(1, "Что такое надёжный пароль?", 1, 1, "SingleChoice") },
        { (1, 2), new QuestionData(2, "Какие признаки фишингового письма?",  2, 1, "MultipleChoice") },
        { (2, 1), new QuestionData(3, "Опишите, что такое фишинг.", 1, 2, "Text") }
    };

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<object>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetAll([FromRoute] int testId, [FromQuery] string? answerType = null)
    {
        // Все вопросы указонного теста
        var query = _questions
            .Where(t => t.Key.Item1 == testId)
            .Select(t => t.Value)
            .AsQueryable();

        // Если передали тип ответа - валидация + фильтрация
        if (!string.IsNullOrWhiteSpace(answerType))
        {
            var allowed = new[] { "SingleChoice", "MultipleChoice", "Text" };
            if (!allowed.Contains(answerType, StringComparer.OrdinalIgnoreCase))
                return BadRequest($"answerType должен быть одним из: {string.Join(", ", allowed)}");

            query = query
                .Where(q => q.AnswerType.Equals(answerType, StringComparison.OrdinalIgnoreCase));
        }

        var result = query.OrderBy(q => q.Number).ToList();

        if (!result.Any()) 
            return NotFound($"Вопросы для теста {testId} не найдены.");

        return Ok(result);
    }

    [HttpGet("{questionId:int}")]
    [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult GetById(
        [FromRoute] int testId,
        [FromRoute] int questionId)
    {
        if (!_questions.TryGetValue((testId, questionId), out var question))
            return NotFound($"Вопрос {questionId} в тесте {testId} не найден.");

        return Ok(question);
    }

    [HttpPost]
    [ProducesResponseType(typeof(object), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    public IActionResult CreateQuestion([FromRoute] int testId, [FromBody] CreateQuestionRequest request)
    {
        var NewId = 4;
        return Created(
            $"/api/tests/{testId}/questions/{NewId}", new QuestionData(NewId, request.Text, request.Number, testId, request.AnswerType));
    }

    [HttpPut("{questionId:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult Update([FromRoute] int testId, [FromRoute] int questionId, [FromBody] UpdateQuestionRequest request)
    {
        if (questionId != request.Id)
            return BadRequest("Идентификатор в пути и в теле не совпадают.");

        if (!_questions.ContainsKey((testId, questionId)))
            return NotFound($"Вопрос {questionId} в тесте {testId} не найден.");

        return NoContent();
    }

    [HttpDelete("{questionId:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public IActionResult Delete([FromRoute] int testId, [FromRoute] int questionId)
    {
        if (!_questions.ContainsKey((testId, questionId)))
            return NotFound($"Вопрос {questionId} в тесте {testId} не найден.");

        return NoContent();
    }


    public record QuestionData(int Id, string Text, int Number, int TestId, string AnswerType);
    public record CreateQuestionRequest(string Text, int Number, string AnswerType);
    public record UpdateQuestionRequest(int Id, string Text, int Number, string AnswerType);
}