using Formit.Application.Interfaces;
using Formit.Shared.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Formit.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class SubmissionController : ControllerBase
{
    private readonly ISubmissionService _submissionService;

    public SubmissionController(ISubmissionService submissionService)
    {
        _submissionService = submissionService;
    }

    [HttpPost]
    [Authorize(Policy = "UserPolicy")]
    public async Task<IActionResult> Submit([FromBody] SubmitFormDto dto)
    {
        var submissionId = await _submissionService.SubmitQuizAsync(dto);
        return Ok(new { SubmissionId = submissionId, Message = "Quiz submitted successfully." });
    }

    [HttpGet("quiz/{quizId}")]
    [Authorize(Policy = "UserPolicy")]
    public async Task<IActionResult> GetSubmissionsByQuiz(int quizId)
    {
        var result = await _submissionService.GetSubmissionsByQuizIdAsync(quizId);
        return Ok(result);
    }

    [HttpGet("{id}")]
    [Authorize(Policy = "UserPolicy")]
    public async Task<IActionResult> GetSubmissionDetails(int id)
    {
        var result = await _submissionService.GetSubmissionDetailsAsync(id);
        return Ok(result);
    }
}
