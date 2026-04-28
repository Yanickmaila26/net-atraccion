using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Servicio.Atraccion.Business.Interfaces;

namespace Servicio.Atraccion.API.Controllers.V1;

[ApiController]
[Route("api/v1/[controller]")]
[Authorize(Roles = "Admin,Partner")] // Solo admins y partners pueden subir fotos
public class MediaController : ControllerBase
{
    private readonly IStorageService _storageService;

    public MediaController(IStorageService storageService)
    {
        _storageService = storageService;
    }

    /// <summary>
    /// Sube un archivo de imagen (form-data) y devuelve la URL estática/pública generada.
    /// Esta URL luego se inyecta en el JSON de CreateCompleteAttractionRequest.
    /// </summary>
    /// <param name="file">Archivo de imagen (ej. jpg, png)</param>
    /// <returns>Objeto JSON con la propiedad 'url'</returns>
    [HttpPost("upload")]
    public async Task<ActionResult> UploadImage(IFormFile file)
    {
        if (file == null || file.Length == 0)
        {
            return BadRequest(new { message = "No se envió ningún archivo o el archivo está vacío." });
        }

        try
        {
            // Opcional: Validar extensiones permitidas (jpeg, png, webp, etc.)
            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".webp" };
            var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
            
            if (!allowedExtensions.Contains(extension))
            {
                return BadRequest(new { message = "Solo se permiten archivos de imagen (.jpg, .png, .webp)." });
            }

            // Opcional: Validar tamaño máximo (ej. 5MB)
            if (file.Length > 5 * 1024 * 1024)
            {
                return BadRequest(new { message = "El archivo excede el tamaño máximo permitido de 5MB." });
            }

            // Delegamos el guardado físico a nuestro StorageService genérico.
            // Hoy guarda en local (/wwwroot/uploads), mañana en Azure Blob.
            var fileUrl = await _storageService.SaveFileAsync(file, "attractions");

            return Ok(new { url = fileUrl });
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Error interno al guardar la imagen.", details = ex.Message });
        }
    }
}
