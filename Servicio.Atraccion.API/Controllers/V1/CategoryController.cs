using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Servicio.Atraccion.Business.DTOs.Category;
using Servicio.Atraccion.Business.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Servicio.Atraccion.API.Controllers.V1;

[ApiController]
[Route("api/v1/[controller]")]
public class CategoryController : ControllerBase
{
    private readonly ICategoryService _categoryService;

    public CategoryController(ICategoryService categoryService)
    {
        _categoryService = categoryService;
    }

    // ─── CATEGORÍAS ───────────────────────────────────────────────────────────

    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<CategoryResponse>>> GetAll()
    {
        var result = await _categoryService.GetAllAsync();
        return Ok(result);
    }

    [HttpGet("{id:guid}")]
    [AllowAnonymous]
    public async Task<ActionResult<CategoryResponse>> GetById(Guid id)
    {
        var result = await _categoryService.GetByIdAsync(id);
        if (result == null) return NotFound();
        return Ok(result);
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<Guid>> CreateCategory([FromBody] CreateCategoryRequest request)
    {
        var id = await _categoryService.CreateCategoryAsync(request);
        return StatusCode(201, new { id, message = "Categoría creada con éxito." });
    }

    [HttpPut("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> UpdateCategory(Guid id, [FromBody] CreateCategoryRequest request)
    {
        var success = await _categoryService.UpdateCategoryAsync(id, request);
        if (!success) return NotFound();
        return Ok(new { message = "Categoría actualizada con éxito." });
    }

    [HttpDelete("{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> DeleteCategory(Guid id)
    {
        var success = await _categoryService.DeleteCategoryAsync(id);
        if (!success) return NotFound();
        return Ok(new { message = "Categoría eliminada con éxito." });
    }

    // ─── SUBCATEGORÍAS ────────────────────────────────────────────────────────

    [HttpGet("subcategory")]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<SubcategoryResponse>>> GetAllSubcategories()
    {
        var all = await _categoryService.GetAllAsync();
        var subcategories = all.SelectMany(c => c.Subcategories);
        return Ok(subcategories);
    }

    [HttpGet("{categoryId:guid}/subcategory")]
    [AllowAnonymous]
    public async Task<ActionResult<IEnumerable<SubcategoryResponse>>> GetSubcategoriesByCategory(Guid categoryId)
    {
        var category = await _categoryService.GetByIdAsync(categoryId);
        if (category == null) return NotFound();
        return Ok(category.Subcategories);
    }

    [HttpPost("subcategory")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult<Guid>> CreateSubcategory([FromBody] CreateSubcategoryRequest request)
    {
        var id = await _categoryService.CreateSubcategoryAsync(request);
        return StatusCode(201, new { id, message = "Subcategoría creada con éxito." });
    }

    [HttpPut("subcategory/{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> UpdateSubcategory(Guid id, [FromBody] CreateSubcategoryRequest request)
    {
        var success = await _categoryService.UpdateSubcategoryAsync(id, request);
        if (!success) return NotFound();
        return Ok(new { message = "Subcategoría actualizada con éxito." });
    }

    [HttpDelete("subcategory/{id:guid}")]
    [Authorize(Roles = "Admin")]
    public async Task<ActionResult> DeleteSubcategory(Guid id)
    {
        var success = await _categoryService.DeleteSubcategoryAsync(id);
        if (!success) return NotFound();
        return Ok(new { message = "Subcategoría eliminada con éxito." });
    }
}
