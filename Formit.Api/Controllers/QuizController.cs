using Formit.Application.Interfaces;
using Formit.Shared.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Formit.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class QuizController : ControllerBase
{
    private readonly IQuizService _quizService;

    public QuizController(IQuizService quizService)
    {
        _quizService = quizService;
    }

    [HttpGet]
    [Authorize(Policy = "UserPolicy")]
    public async Task<IActionResult> GetAll(
        [FromQuery] int page = 1,
        [FromQuery] int pageSize = 10,
        [FromQuery] string? title = null)
    {
        if (page < 1)
            page = 1;
        if (pageSize < 1 || pageSize > 50)
            pageSize = 10;

        var result = await _quizService.GetAllPagedAsync(page, pageSize, title);

        return Ok(result);
    }

    [HttpGet("{id}")]
    [Authorize(Policy = "UserPolicy")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _quizService.GetByIdAsync(id);
        return Ok(result);
    }

    [HttpPost]
    [Authorize(Policy = "AdminPolicy")]
    public async Task<IActionResult> Create([FromBody] CreateQuizDto dto)
    {
        var result = await _quizService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id}")]
    [Authorize(Policy = "AdminPolicy")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateQuizDto dto)
    {
        var result = await _quizService.UpdateAsync(id, dto);
        return Ok(result);
    }

    [HttpDelete("{id}")]
    [Authorize(Policy = "AdminPolicy")]
    public async Task<IActionResult> Delete(int id)
    {
        await _quizService.DeleteAsync(id);
        return Ok();
    }
}