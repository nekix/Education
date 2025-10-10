using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Text;
using System.Text.Json;
using CarPark.Identity;
using CarPark.ManagersOperations;
using CarPark.ManagersOperations.ExportImport;
using CarPark.ManagersOperations.ExportImport.Commands;
using CarPark.Shared.CQ;
using CsvHelper;
using FluentResults;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarPark.Controllers.Api.Controllers;

[Authorize(AppIdentityConst.ManagerPolicy)]
public class ImportController : ApiBaseController
{
    private readonly ICommandHandler<ImportCommand, Result> _importCommandHandler;

    public ImportController(ICommandHandler<ImportCommand, Result> importCommandHandler)
    {
        _importCommandHandler = importCommandHandler;
    }

    // POST: /api/import/enterprises
    [HttpPost("enterprises")]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> ImportEnterprises(IFormFile file)
    {
        int managerId = GetCurrentManagerId();

        List<EnterpriseExportImportDto> enterprises;

        await using Stream fs = file.OpenReadStream();

        if (IsCsvContent(file.ContentType))
        {
            enterprises = await ParseCsv<EnterpriseExportImportDto>(fs);
        }
        else
        {
            ImportEnterprisesRequest request = await ParseJson<ImportEnterprisesRequest>(fs);
            enterprises = request.Enterprises;
        }

        ImportCommand command = new ImportCommand
        {
            RequestingManagerId = managerId,
            Enterprises = enterprises,
            Models = null,
            Vehicles = null,
            Rides = null,
            Tracks = null
        };

        Result importResult = await _importCommandHandler.Handle(command);

        if (importResult.IsSuccess)
        {
            return NoContent();
        }

        if (importResult.HasError(e => e.Message == ManagersOperationsErrors.ManagerNotExist))
        {
            return Forbid();
        }
        else if (importResult.HasError(e => e.Message == ExportImportHandlerErrors.TimeZoneNotExist))
        {
            return BadRequest("TimeZone not found");
        }
        else if (importResult.HasError(e => e.Message == ExportImportHandlerErrors.ManagerNotAllowedToEnterprise))
        {
            return Forbid();
        }
        else
        {
            return BadRequest();
        }
    }

    // POST: /api/import/models
    [HttpPost("models")]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    public async Task<IActionResult> ImportModels(IFormFile file)
    {
        int managerId = GetCurrentManagerId();

        List<ModelExportImportDto> models;

        await using Stream fs = file.OpenReadStream();

        if (IsCsvContent(file.ContentType))
        {
            models = await ParseCsv<ModelExportImportDto>(fs);
        }
        else
        {
            ImportModelsRequest request = await ParseJson<ImportModelsRequest>(fs);
            models = request.Models;
        }

        ImportCommand command = new ImportCommand
        {
            RequestingManagerId = managerId,
            Enterprises = null,
            Models = models,
            Vehicles = null,
            Rides = null,
            Tracks = null
        };

        Result importResult = await _importCommandHandler.Handle(command);

        if (importResult.IsSuccess)
        {
            return NoContent();
        }

        if (importResult.HasError(e => e.Message == ManagersOperationsErrors.ManagerNotExist))
        {
            return Forbid();
        }
        else
        {
            return BadRequest();
        }
    }

    // POST: /api/import/vehicles
    [HttpPost("vehicles")]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ImportVehicles(IFormFile file)
    {
        int managerId = GetCurrentManagerId();

        List<VehicleExportImportDto> vehicles;

        await using Stream fs = file.OpenReadStream();

        if (IsCsvContent(file.ContentType))
        {
            vehicles = await ParseCsv<VehicleExportImportDto>(fs);
        }
        else
        {
            
            ImportVehiclesRequest request = await ParseJson<ImportVehiclesRequest>(fs);
            vehicles = request.Vehicles;
        }

        ImportCommand command = new ImportCommand
        {
            RequestingManagerId = managerId,
            Enterprises = null,
            Models = null,
            Vehicles = vehicles,
            Rides = null,
            Tracks = null
        };

        Result importResult = await _importCommandHandler.Handle(command);

        if (importResult.IsSuccess)
        {
            return NoContent();
        }

        if (importResult.HasError(e => e.Message == ManagersOperationsErrors.ManagerNotExist))
        {
            return Forbid();
        }
        else if (importResult.HasError(e => e.Message == ExportImportHandlerErrors.EnterpriseNotFound))
        {
            return NotFound("Enterprise not found");
        }
        else if (importResult.HasError(e => e.Message == ExportImportHandlerErrors.ModelNotFound))
        {
            return NotFound("Model not found");
        }
        else if (importResult.HasError(e => e.Message == ExportImportHandlerErrors.ManagerNotAllowedToEnterprise))
        {
            return Forbid();
        }
        else
        {
            return BadRequest();
        }
    }

    // POST: /api/import/rides
    [HttpPost("rides")]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ImportRides(IFormFile file)
    {
        int managerId = GetCurrentManagerId();

        List<VehicleRideExportImportDto> rides;

        await using Stream fs = file.OpenReadStream();

        if (IsCsvContent(file.ContentType))
        {
            rides = await ParseCsv<VehicleRideExportImportDto>(fs);
        }
        else
        {
            ImportRidesRequest request = await ParseJson<ImportRidesRequest>(fs);
            rides = request.Rides;
        }

        ImportCommand command = new ImportCommand
        {
            RequestingManagerId = managerId,
            Enterprises = null,
            Models = null,
            Vehicles = null,
            Rides = rides,
            Tracks = null
        };

        Result importResult = await _importCommandHandler.Handle(command);

        if (importResult.IsSuccess)
        {
            return NoContent();
        }

        if (importResult.HasError(e => e.Message == ManagersOperationsErrors.ManagerNotExist))
        {
            return Forbid();
        }
        else if (importResult.HasError(e => e.Message == ExportImportHandlerErrors.VehicleNotExist))
        {
            return NotFound("Vehicle not found");
        }
        else if (importResult.HasError(e => e.Message == ExportImportHandlerErrors.RidePointNotFound))
        {
            return NotFound("Ride point not found");
        }
        else if (importResult.HasError(e => e.Message == ExportImportHandlerErrors.ManagerNotAllowedToEnterprise))
        {
            return Forbid();
        }
        else
        {
            return BadRequest();
        }
    }

    // POST: /api/import/tracks
    [HttpPost("tracks")]
    [Consumes("multipart/form-data")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ImportTracks(IFormFile file)
    {
        int managerId = GetCurrentManagerId();

        List<VehicleGeoTimePointExportImportDto> tracks;

        await using Stream fs = file.OpenReadStream();

        if (IsCsvContent(file.ContentType))
        {
            tracks = await ParseCsv<VehicleGeoTimePointExportImportDto>(fs);
        }
        else
        {
            ImportTracksRequest request = await ParseJson<ImportTracksRequest>(fs);
            tracks = request.Tracks;
        }

        ImportCommand command = new ImportCommand
        {
            RequestingManagerId = managerId,
            Enterprises = null,
            Models = null,
            Vehicles = null,
            Rides = null,
            Tracks = tracks
        };

        Result importResult = await _importCommandHandler.Handle(command);

        if (importResult.IsSuccess)
        {
            return NoContent();
        }

        if (importResult.HasError(e => e.Message == ManagersOperationsErrors.ManagerNotExist))
        {
            return Forbid();
        }
        else if (importResult.HasError(e => e.Message == ExportImportHandlerErrors.VehicleNotExist))
        {
            return NotFound("Vehicle not found");
        }
        else if (importResult.HasError(e => e.Message == ExportImportHandlerErrors.ManagerNotAllowedToEnterprise))
        {
            return Forbid();
        }
        else
        {
            return BadRequest();
        }
    }

    // Request models
    public class ImportEnterprisesRequest
    {
        [Required]
        public required List<EnterpriseExportImportDto> Enterprises { get; set; }
    }

    public class ImportModelsRequest
    {
        [Required]
        public required List<ModelExportImportDto> Models { get; set; }
    }

    public class ImportVehiclesRequest
    {
        [Required]
        public required List<VehicleExportImportDto> Vehicles { get; set; }
    }

    public class ImportRidesRequest
    {
        [Required]
        public required List<VehicleRideExportImportDto> Rides { get; set; }
    }

    public class ImportTracksRequest
    {
        [Required]
        public required List<VehicleGeoTimePointExportImportDto> Tracks { get; set; }
    }

    private async Task<List<T>> ParseCsv<T>(Stream fs)
    {
        using StreamReader reader = new StreamReader(fs, Encoding.UTF8);
        using CsvReader csv = new CsvReader(reader, CultureInfo.InvariantCulture);
        csv.Context.TypeConverterOptionsCache.GetOptions<string?>().NullValues.Add(string.Empty);

        List<T> records = new List<T>();

        await foreach (T item in csv.GetRecordsAsync<T>())
            records.Add(item);

        return records;
    }

    private async Task<T> ParseJson<T>(Stream fs)
    {
        using StreamReader reader = new StreamReader(fs, Encoding.UTF8);

        T? data = await System.Text.Json.JsonSerializer.DeserializeAsync<T>(fs, JsonSerializerOptions.Web);

        if (data == null)
            throw new InvalidOperationException("Failed to parse JSON data");

        return data;
    }

    private bool IsCsvContent(string contentType)
    {
        return contentType.Contains("text/csv") ||
               contentType.Contains("application/csv") ||
               contentType.Contains("text/comma-separated-values") ||
               contentType.Contains("application/vnd.ms-excel") ||
               contentType.Contains("text/x-csv") ||
               contentType.Contains("application/x-csv");
    }
}