using Servicio.Atraccion.Business.DTOs.Cliente;
using Servicio.Atraccion.DataManagement.Models;

namespace Servicio.Atraccion.Business.Mappers;

public static class ClienteBusinessMapper
{
    public static ClientNode ToDataModel(this CrearClienteRequest request)
    {
        return new ClientNode
        {
            FirstName = request.FirstName,
            LastName = request.LastName,
            Phone = request.Phone,
            DocumentNumber = request.Identification
        };
    }

    public static ClienteResponse ToResponse(this ClientNode dataModel)
    {
        return new ClienteResponse
        {
            Id = dataModel.Id,
            Identification = dataModel.DocumentNumber ?? string.Empty,
            FullName = $"{dataModel.FirstName} {dataModel.LastName}",
            Email = dataModel.UserEmail,
            Phone = dataModel.Phone
        };
    }

    public static void ApplyToModel(this ActualizarClienteRequest request, ClientNode model)
    {
        model.FirstName = request.FirstName;
        model.LastName = request.LastName;
        model.Phone = request.Phone;
        model.BirthDate = request.BirthDate;
        model.Nationality = request.Nationality;
        model.LocationId = request.LocationId;
        model.PreferredLang = request.PreferredLang;
    }
}