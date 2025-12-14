using Formit.Application.Interfaces;
using Formit.Shared.DTOs;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Formit.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class OptionController : ControllerBase
{
    private readonly IOptionService _optionService;

    public OptionController(IOptionService optionService)
    {
        _optionService = optionService;
    }

    [HttpPost("question/{questionId}")]
    [Authorize(Policy = "AdminPolicy")]
    public async Task<IActionResult> Create(int questionId, [FromBody] CreateOptionDto dto)
    {
        var result = await _optionService.CreateAsync(questionId, dto);
        return Ok(result);
    }

    [HttpPut("{id}")]
    [Authorize(Policy = "AdminPolicy")]
    public async Task<IActionResult> Update(int id, [FromBody] UpdateOptionDto dto)
    {
        var result = await _optionService.UpdateAsync(id, dto);
        return Ok(result);
    }

    [HttpDelete("{id}")]
    [Authorize(Policy = "AdminPolicy")]
    public async Task<IActionResult> Delete(int id)
    {
        await _optionService.DeleteAsync(id);
        return NoContent();
    }
}
