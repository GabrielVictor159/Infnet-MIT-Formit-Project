using Formit.Application.Interfaces;
using Formit.Shared.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Formit.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class QuestionController : ControllerBase
{
    private readonly IQuestionService _questionService;

    public QuestionController(IQuestionService questionService)
    {
        _questionService = questionService;
    }

    [HttpGet("{id}")]
    [Authorize(Policy = "UserPolicy")]
    public async Task<IActionResult> GetById(int id)
    {
        var result = await _questionService.GetByIdAsync(id);
        return Ok(result);
    }

    [HttpPost("quiz/{quizId}")]
    [Authorize(Policy = "AdminPolicy")]
    public async Task<IActionResult> Create(int quizId, [FromBody] CreateQuestionDto dto)
    {
        var result = await _questionService.CreateAsync(quizId, dto);
        return CreatedAtAction(nameof(GetById), new { id = result.Id }, result);
    }

    [HttpPut("{id}")]
    [Authorize(Policy = "AdminPolicy")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateQuestionDto dto)
    {
        var result = await _questionService.UpdateAsync(id, dto);
        return Ok(result);
    }

    [HttpDelete("{id}")]
    [Authorize(Policy = "AdminPolicy")]
    public async Task<IActionResult> Delete(int id)
    {
        await _questionService.DeleteAsync(id);
        return NoContent();
    }
}
