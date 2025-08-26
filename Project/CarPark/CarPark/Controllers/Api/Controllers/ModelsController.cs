using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CarPark.Data;
using CarPark.Attributes;
using CarPark.Models.Models;
using CarPark.Shared.CQ;
using FluentResults;
using Microsoft.Build.Framework;

namespace CarPark.Controllers.Api.Controllers;

public class ModelsController : ApiBaseController
{
    private readonly ICommandHandler<CreateModelCommand, Result<int>> _createModelHandler;
    private readonly ICommandHandler<DeleteModelCommand, Result> _deleteModelHandler;
    private readonly ICommandHandler<UpdateModelCommand, Result<int>> _editModelHandler;

    private readonly IModelsDbSet _set;

    public ModelsController(IModelsDbSet set,
        ICommandHandler<CreateModelCommand, Result<int>> createModelHandler,
        ICommandHandler<DeleteModelCommand, Result> deleteModelHandler,
        ICommandHandler<UpdateModelCommand, Result<int>> editModelHandler)
    {
        _set = set;
        _createModelHandler = createModelHandler;
        _deleteModelHandler = deleteModelHandler;
        _editModelHandler = editModelHandler;
    }

    // GET: api/Models
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<Model>))]
    public async Task<ActionResult<IEnumerable<Model>>> GetModels()
    {
        return Ok(await _set.Models.ToListAsync());
    }

    // GET: api/Models/5
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(Model))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<Model>> GetModel(int id)
    {
        var model = await _set.Models.FindAsync(id);

        if (model == null)
        {
            return NotFound();
        }

        return model;
    }

    // PUT: api/Models/5
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPut("{id}")]
    [AppValidateAntiForgeryToken]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> PutModel(int id, CreateUpdateModelRequest request)
    {
        UpdateModelCommand command = new UpdateModelCommand
        {
            Id = id,
            ModelName = request.ModelName,
            VehicleType = request.VehicleType,
            SeatsCount = request.SeatsCount,
            MaxLoadingWeightKg = request.MaxLoadingWeightKg,
            EnginePowerKW = request.EnginePowerKW,
            TransmissionType = request.TransmissionType,
            FuelSystemType = request.FuelSystemType,
            FuelTankVolumeLiters = request.FuelTankVolumeLiters
        };

        Result<int> result = await _editModelHandler.Handle(command);

        // Success flow
        if (!result.IsFailed)
            return NoContent();

        // Errors handling
        if (result.HasError(e => e.Message == UpdateModelCommand.Errors.NotFound))
        {
            return NotFound();
        }

        // Undefined errors
        return BadRequest();
    }

    // POST: api/Models
    // To protect from overposting attacks, see https://go.microsoft.com/fwlink/?linkid=2123754
    [HttpPost]
    [AppValidateAntiForgeryToken]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<Model>> PostModel(CreateUpdateModelRequest request)
    {
        CreateModelCommand command = new CreateModelCommand
        {
            ModelName = request.ModelName,
            VehicleType = request.VehicleType,
            SeatsCount = request.SeatsCount,
            MaxLoadingWeightKg = request.MaxLoadingWeightKg,
            EnginePowerKW = request.EnginePowerKW,
            TransmissionType = request.TransmissionType,
            FuelSystemType = request.FuelSystemType,
            FuelTankVolumeLiters = request.FuelTankVolumeLiters
        };

        Result<int> result = await _createModelHandler.Handle(command);

        // Success flow
        if (result.IsSuccess)
            return CreatedAtAction("GetModel", new { id = result.Value }, null);

        // Undefined errors
        return BadRequest();
    }

    // DELETE: api/Models/5
    [HttpDelete("{id}")]
    [AppValidateAntiForgeryToken]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> DeleteModel(int id)
    {
        DeleteModelCommand command = new DeleteModelCommand { Id = id };
        
        Result result = await _deleteModelHandler.Handle(command);

        // Success flow
        if (!result.IsFailed) 
            return NoContent();

        // Errors handling
        if (result.HasError(e => e.Message == DeleteModelCommand.Errors.NotFound))
        {
            return NotFound();
        }

        if (result.HasError(e => e.Message == DeleteModelCommand.Errors.Conflict))
        {
            return Conflict();
        }

        // Undefined errors
        return BadRequest();
    }

    public class CreateUpdateModelRequest
    {
        [Required]
        public required string ModelName { get; set; }

        [Required]
        public required string VehicleType { get; set; }

        [Required]
        public required int SeatsCount { get; set; }

        [Required]
        public required double MaxLoadingWeightKg { get; set; }

        [Required]
        public required double EnginePowerKW { get; set; }

        [Required]
        public required string TransmissionType { get; set; }

        [Required]
        public required string FuelSystemType { get; set; }
        
        [Required]
        public required string FuelTankVolumeLiters { get; set; }
    }
}