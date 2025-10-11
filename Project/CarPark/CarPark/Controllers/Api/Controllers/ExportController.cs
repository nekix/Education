using CarPark.Identity;
using CarPark.ManagersOperations;
using CarPark.ManagersOperations.ExportImport;
using CarPark.ManagersOperations.ExportImport.Queries;
using CarPark.Shared.CQ;
using CsvHelper;
using FluentResults;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Text;
using System.Text.Json;

namespace CarPark.Controllers.Api.Controllers;

[Authorize(AppIdentityConst.ManagerPolicy)]
public class ExportController : ApiBaseController
{
    private readonly IQueryHandler<ExportEnterpriseQuery, Result<EnterpriseExportImportDto>> _exportEnterpriseHandler;
    private readonly IQueryHandler<ExportEnterpriseVehiclesQuery, Result<List<VehicleExportImportDto>>> _exportEnterpriseVehiclesHandler;
    private readonly IQueryHandler<ExportVehicleRidesQuery, Result<List<VehicleRideExportImportDto>>> _exportVehicleRidesHandler;
    private readonly IQueryHandler<ExportVehicleTrackQuery, Result<List<VehicleGeoTimePointExportImportDto>>> _exportVehicleTrackHandler;
    private readonly IQueryHandler<ExportModelsQuery, Result<List<ModelExportImportDto>>> _exportModelsHandler;

    public ExportController(
        IQueryHandler<ExportEnterpriseQuery, Result<EnterpriseExportImportDto>> exportEnterpriseHandler,
        IQueryHandler<ExportEnterpriseVehiclesQuery, Result<List<VehicleExportImportDto>>> exportEnterpriseVehiclesHandler,
        IQueryHandler<ExportVehicleRidesQuery, Result<List<VehicleRideExportImportDto>>> exportVehicleRidesHandler,
        IQueryHandler<ExportVehicleTrackQuery, Result<List<VehicleGeoTimePointExportImportDto>>> exportVehicleTrackHandler,
        IQueryHandler<ExportModelsQuery, Result<List<ModelExportImportDto>>> exportModelsHandler)
    {
        _exportEnterpriseHandler = exportEnterpriseHandler;
        _exportEnterpriseVehiclesHandler = exportEnterpriseVehiclesHandler;
        _exportVehicleRidesHandler = exportVehicleRidesHandler;
        _exportVehicleTrackHandler = exportVehicleTrackHandler;
        _exportModelsHandler = exportModelsHandler;
    }

    [HttpGet("enterprises/{id}")]
    [Produces("application/json", "text/csv")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ExportEnterprise(Guid id)
    {
        Guid managerId = GetCurrentManagerId();

        ExportEnterpriseQuery query = new ExportEnterpriseQuery
        {
            RequestingManagerId = managerId,
            EnterpriseId = id
        };

        Result<EnterpriseExportImportDto> exportEnterprise = await _exportEnterpriseHandler.Handle(query);

        if (exportEnterprise.IsSuccess)
        {
            List<EnterpriseExportImportDto> enterprises = new List<EnterpriseExportImportDto> { exportEnterprise.Value };

            if (IsCsvRequested())
            {
                return CreateCsvFile(enterprises, "enterprises");
            }
            else
            {
                EnterprisesExportResponse response = new EnterprisesExportResponse
                {
                    Enterprises = enterprises
                };

                return CreateJsonFile(response, "enterprises");
            }
        }

        if (exportEnterprise.HasError(e => e.Message == ManagersOperationsErrors.ManagerNotExist))
        {
            return Forbid();
        }
        else if (exportEnterprise.HasError(e => e.Message == ExportImportHandlerErrors.ManagerNotAllowedToEnterprise))
        {
            return Forbid();
        }
        else
        {
            return BadRequest();
        }
    }

    [HttpGet("enterprises/{id}/vehicles")]
    [Produces("application/json", "text/csv")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ExportEnterpriseVehicles(Guid id)
    {
        Guid managerId = GetCurrentManagerId();

        ExportEnterpriseVehiclesQuery query = new ExportEnterpriseVehiclesQuery
        {
            RequestingManagerId = managerId,
            EnterpriseId = id
        };

        Result<List<VehicleExportImportDto>> exportVehicles = await _exportEnterpriseVehiclesHandler.Handle(query);

        if (exportVehicles.IsSuccess)
        {
            if (IsCsvRequested())
            {
                return CreateCsvFile(exportVehicles.Value, "vehicles");
            }
            else
            {
                VehiclesExportResponse response = new VehiclesExportResponse
                {
                    Vehicles = exportVehicles.Value
                };

                return CreateJsonFile(response, "vehicles");
            }
        }

        if (exportVehicles.HasError(e => e.Message == ManagersOperationsErrors.ManagerNotExist))
        {
            return Forbid();
        }
        else if (exportVehicles.HasError(e => e.Message == ExportImportHandlerErrors.ManagerNotAllowedToEnterprise))
        {
            return Forbid();
        }
        else
        {
            return BadRequest();
        }
    }

    [HttpGet("vehicles/{id}/rides")]
    [Produces("application/json", "text/csv")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ExportVehicleRides(Guid id, [FromQuery] ExportVehicleRidesRequest request)
    {
        Guid managerId = GetCurrentManagerId();

        ExportVehicleRidesQuery query = new ExportVehicleRidesQuery
        {
            RequestingManagerId = managerId,
            VehicleId = id,
            RidesStartTime = request.StartTime,
            RidesEndTime = request.EndTime
        };

        Result<List<VehicleRideExportImportDto>> exportRides = await _exportVehicleRidesHandler.Handle(query);

        if (exportRides.IsSuccess)
        {
            if (IsCsvRequested())
            {
                return CreateCsvFile(exportRides.Value, "rides");
            }
            else
            {
                RidesExportResponse response = new RidesExportResponse
                {
                    Rides = exportRides.Value
                };

                return CreateJsonFile(response, "rides");
            }
        }

        if (exportRides.HasError(e => e.Message == ManagersOperationsErrors.ManagerNotExist))
        {
            return Forbid();
        }
        else if (exportRides.HasError(e => e.Message == ExportImportHandlerErrors.VehicleNotExist))
        {
            return NotFound();
        }
        else
        {
            return BadRequest();
        }
    }

    // GET: /api/export/models
    [HttpGet("models")]
    [Produces("application/json", "text/csv")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> ExportModels()
    {
        Guid managerId = GetCurrentManagerId();

        ExportModelsQuery query = new ExportModelsQuery
        {
            RequestingManagerId = managerId
        };

        Result<List<ModelExportImportDto>> exportModels = await _exportModelsHandler.Handle(query);

        if (exportModels.IsSuccess)
        {
            if (IsCsvRequested())
            {
                return CreateCsvFile(exportModels.Value, "models");
            }
            else
            {
                ModelsExportResponse response = new ModelsExportResponse
                {
                    Models = exportModels.Value
                };

                return CreateJsonFile(response, "models");
            }
        }

        if (exportModels.HasError(e => e.Message == ManagersOperationsErrors.ManagerNotExist))
        {
            return Forbid();
        }
        else
        {
            return BadRequest();
        }
    }

    // GET: /api/export/vehicles/{id}/tracks
    [HttpGet("vehicles/{id}/tracks")]
    [Produces("application/json", "text/csv")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> ExportVehicleTrack(Guid id, [FromQuery] ExportVehicleTrackRequest request)
    {
        Guid managerId = GetCurrentManagerId();

        ExportVehicleTrackQuery query = new ExportVehicleTrackQuery
        {
            RequestingManagerId = managerId,
            VehicleId = id,
            TrackStartTime = request.StartTime,
            TrackEndTime = request.EndTime
        };

        Result<List<VehicleGeoTimePointExportImportDto>> exportTracks = await _exportVehicleTrackHandler.Handle(query);

        if (exportTracks.IsSuccess)
        {
            if (IsCsvRequested())
            {
                return CreateCsvFile(exportTracks.Value, "tracks");
            }
            else
            {
                TracksExportResponse response = new TracksExportResponse
                {
                    Tracks = exportTracks.Value
                };

                return CreateJsonFile(response, "tracks");
            }
        }

        if (exportTracks.HasError(e => e.Message == ManagersOperationsErrors.ManagerNotExist))
        {
            return Forbid();
        }
        else if (exportTracks.HasError(e => e.Message == ExportImportHandlerErrors.VehicleNotExist))
        {
            return NotFound();
        }
        else if (exportTracks.HasError(e => e.Message == ExportImportHandlerErrors.ManagerNotAllowedToEnterprise))
        {
            return Forbid();
        }
        else
        {
            return BadRequest();
        }
    }

    public class ExportVehicleTrackRequest
    {
        [Required]
        public DateTimeOffset StartTime { get; set; }

        [Required]
        public DateTimeOffset EndTime { get; set; }
    }

    public class ExportVehicleRidesRequest
    {
        [Required]
        public DateTimeOffset StartTime { get; set; }

        [Required]
        public DateTimeOffset EndTime { get; set; }
    }

    public class EnterprisesExportResponse
    {
        public required List<EnterpriseExportImportDto> Enterprises { get; set; }
    }

    public class VehiclesExportResponse
    {
        public required List<VehicleExportImportDto> Vehicles { get; set; }
    }

    public class RidesExportResponse
    {
        public required List<VehicleRideExportImportDto> Rides { get; set; }
    }

    public class TracksExportResponse
    {
        public required List<VehicleGeoTimePointExportImportDto> Tracks { get; set; }
    }

    public class ModelsExportResponse
    {
        public required List<ModelExportImportDto> Models { get; set; }
    }

    // Helper methods for file export
    private string GenerateFileName(string entityType, string format)
    {
        string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
        return $"{entityType}_export_{timestamp}.{format}";
    }

    private bool IsCsvRequested()
    {
        string acceptHeader = Request.Headers.Accept.ToString();
        return acceptHeader.Contains("text/csv");
    }

    private string GenerateCsvContent<T>(IEnumerable<T> data)
    {
        using var stringWriter = new StringWriter();
        using var csv = new CsvWriter(stringWriter, CultureInfo.InvariantCulture);

        csv.WriteRecords(data);
        return stringWriter.ToString();
    }

    private FileResult CreateCsvFile<T>(IEnumerable<T> data, string entityType)
    {
        string csvContent = GenerateCsvContent(data);
        string fileName = GenerateFileName(entityType, "csv");
        return File(Encoding.UTF8.GetBytes(csvContent), "text/csv", fileName);
    }

    private FileResult CreateJsonFile(object data, string entityType)
    {
        string jsonContent = JsonSerializer.Serialize(data, options: JsonSerializerOptions.Web);
        string fileName = GenerateFileName(entityType, "json");
        return File(Encoding.UTF8.GetBytes(jsonContent), "application/json", fileName);
    }
}