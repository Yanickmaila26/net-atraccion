using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Servicio.Atraccion.Business.DTOs.Common;
using Servicio.Atraccion.Business.Interfaces;

namespace Servicio.Atraccion.API.Controllers.V1;

[ApiController]
[Route("api/v1/[controller]")]
[AllowAnonymous] // Catálogos suelen ser públicos o para cualquier usuario autenticado
public class TagController : ControllerBase
{
    private readonly IMasterDataService _masterData;

    public TagController(IMasterDataService masterData)
    {
        _masterData = masterData;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<TagResponse>>> GetAll()
    {
        var result = await _masterData.GetTagsAsync();
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    public async Task<ActionResult<TagResponse>> GetById(Guid id)
    {
        var result = await _masterData.GetTagByIdAsync(id);
        return result == null ? NotFound() : Ok(result);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<Guid>> Create([FromBody] CreateTagRequest request)
    {
        var id = await _masterData.CreateTagAsync(request);
        return CreatedAtAction(nameof(GetById), new { id }, id);
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> Update(Guid id, [FromBody] CreateTagRequest request)
    {
        var success = await _masterData.UpdateTagAsync(id, request);
        return success ? NoContent() : NotFound();
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> Delete(Guid id)
    {
        var success = await _masterData.DeleteTagAsync(id);
        return success ? NoContent() : NotFound();
    }
}

