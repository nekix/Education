using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using CarPark.Controllers.Api.Controllers;
using CarPark.CQ;
using FluentResults;
using CarPark.Data.Interfaces;
using CarPark.Models;
using CarPark.ManagersOperations.Models.Commands;

namespace CarPark.Web.Tests.Controllers;

public class ModelsController_Tests
{
    private readonly IModelsDbSet _mockSet;
    private readonly ICommandHandler<CreateModelCommand, Result<Guid>> _mockCreateHandler;
    private readonly ICommandHandler<DeleteModelCommand, Result> _mockDeleteHandler;
    private readonly ICommandHandler<UpdateModelCommand, Result<Guid>> _mockUpdateHandler;
    private readonly ModelsController _controller;

    public ModelsController_Tests()
    {
        _mockSet = Substitute.For<IModelsDbSet>();
        _mockCreateHandler = Substitute.For<ICommandHandler<CreateModelCommand, Result<Guid>>>();
        _mockDeleteHandler = Substitute.For<ICommandHandler<DeleteModelCommand, Result>>();
        _mockUpdateHandler = Substitute.For<ICommandHandler<UpdateModelCommand, Result<Guid>>>();
        
        _controller = new ModelsController(_mockSet, _mockCreateHandler, _mockDeleteHandler, _mockUpdateHandler);
    }

    [Fact]
    public async Task PostModel_ValidRequest_Returns201Created()
    {
        // Arrange
        ModelsController.CreateUpdateModelRequest request = new ModelsController.CreateUpdateModelRequest
        {
            ModelName = "Test Model",
            VehicleType = "Car",
            SeatsCount = 5,
            MaxLoadingWeightKg = 500.0,
            EnginePowerKW = 100.0,
            TransmissionType = "Manual",
            FuelSystemType = "Gasoline",
            FuelTankVolumeLiters = "50"
        };

        CreateModelCommand expectedCommand = new CreateModelCommand
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

        Guid expectedId = Guid.Parse("12345678-1234-1234-1234-123456789abc");
        _mockCreateHandler.Handle(Arg.Any<CreateModelCommand>()).Returns(Result.Ok(expectedId));

        // Act
        ActionResult<Model> result = await _controller.PostModel(request);

        // Assert
        CreatedAtActionResult? createdAtResult = result.Result as CreatedAtActionResult;
        Assert.True(createdAtResult != null, $"Returned action result must be '{nameof(CreatedAtActionResult)}' but was '{result.Result}'.");
        Assert.NotNull(createdAtResult);
        Assert.Equal(201, createdAtResult.StatusCode);
        Assert.Equal("GetModel", createdAtResult.ActionName);
        Assert.Equal(expectedId, createdAtResult.RouteValues?["id"]);
        
        await _mockCreateHandler.Received(1).Handle(Arg.Is<CreateModelCommand>(cmd => 
            cmd.ModelName == expectedCommand.ModelName &&
            cmd.VehicleType == expectedCommand.VehicleType &&
            cmd.SeatsCount == expectedCommand.SeatsCount &&
            cmd.MaxLoadingWeightKg == expectedCommand.MaxLoadingWeightKg &&
            cmd.EnginePowerKW == expectedCommand.EnginePowerKW &&
            cmd.TransmissionType == expectedCommand.TransmissionType &&
            cmd.FuelSystemType == expectedCommand.FuelSystemType &&
            cmd.FuelTankVolumeLiters == expectedCommand.FuelTankVolumeLiters));
    }

    [Fact]
    public async Task PutModel_ValidRequest_Returns204NoContent()
    {
        // Arrange
        Guid modelId = Guid.Parse("12345678-1234-1234-1234-123456789abc");
        ModelsController.CreateUpdateModelRequest request = new ModelsController.CreateUpdateModelRequest
        {
            ModelName = "Updated Model",
            VehicleType = "Truck",
            SeatsCount = 3,
            MaxLoadingWeightKg = 2000.0,
            EnginePowerKW = 200.0,
            TransmissionType = "Automatic",
            FuelSystemType = "Diesel",
            FuelTankVolumeLiters = "80"
        };

        UpdateModelCommand expectedCommand = new UpdateModelCommand
        {
            Id = modelId,
            ModelName = request.ModelName,
            VehicleType = request.VehicleType,
            SeatsCount = request.SeatsCount,
            MaxLoadingWeightKg = request.MaxLoadingWeightKg,
            EnginePowerKW = request.EnginePowerKW,
            TransmissionType = request.TransmissionType,
            FuelSystemType = request.FuelSystemType,
            FuelTankVolumeLiters = request.FuelTankVolumeLiters
        };

        _mockUpdateHandler.Handle(Arg.Any<UpdateModelCommand>()).Returns(Result.Ok(modelId));

        // Act
        IActionResult result = await _controller.PutModel(modelId, request);

        // Assert
        NoContentResult? noContentResult = result as NoContentResult;
        Assert.NotNull(noContentResult);
        Assert.Equal(204, noContentResult.StatusCode);
        
        await _mockUpdateHandler.Received(1).Handle(Arg.Is<UpdateModelCommand>(cmd => 
            cmd.Id == expectedCommand.Id &&
            cmd.ModelName == expectedCommand.ModelName &&
            cmd.VehicleType == expectedCommand.VehicleType &&
            cmd.SeatsCount == expectedCommand.SeatsCount &&
            cmd.MaxLoadingWeightKg == expectedCommand.MaxLoadingWeightKg &&
            cmd.EnginePowerKW == expectedCommand.EnginePowerKW &&
            cmd.TransmissionType == expectedCommand.TransmissionType &&
            cmd.FuelSystemType == expectedCommand.FuelSystemType &&
            cmd.FuelTankVolumeLiters == expectedCommand.FuelTankVolumeLiters));
    }

    [Fact]
    public async Task DeleteModel_ValidRequest_Returns204NoContent()
    {
        // Arrange
        Guid modelId = Guid.Parse("12345678-1234-1234-1234-123456789abc");
        DeleteModelCommand expectedCommand = new DeleteModelCommand { Id = modelId };

        _mockDeleteHandler.Handle(Arg.Any<DeleteModelCommand>()).Returns(Result.Ok());

        // Act
        IActionResult result = await _controller.DeleteModel(modelId);

        // Assert
        NoContentResult? noContentResult = result as NoContentResult;
        Assert.NotNull(noContentResult);
        Assert.Equal(204, noContentResult.StatusCode);
        
        await _mockDeleteHandler.Received(1).Handle(Arg.Is<DeleteModelCommand>(cmd => cmd.Id == expectedCommand.Id));
    }

    [Fact]
    public async Task PostModel_HandlerFails_Returns400BadRequest()
    {
        // Arrange
        ModelsController.CreateUpdateModelRequest request = new ModelsController.CreateUpdateModelRequest
        {
            ModelName = "Test Model",
            VehicleType = "Car",
            SeatsCount = 5,
            MaxLoadingWeightKg = 500.0,
            EnginePowerKW = 100.0,
            TransmissionType = "Manual",
            FuelSystemType = "Gasoline",
            FuelTankVolumeLiters = "50"
        };

        _mockCreateHandler.Handle(Arg.Any<CreateModelCommand>()).Returns(Result.Fail("Validation error"));

        // Act
        ActionResult<Model> result = await _controller.PostModel(request);

        // Assert
        BadRequestResult? badRequestResult = result.Result as BadRequestResult;
        Assert.NotNull(badRequestResult);
        Assert.Equal(400, badRequestResult.StatusCode);
    }

    [Fact]
    public async Task PutModel_HandlerFails_Returns400BadRequest()
    {
        // Arrange
        Guid modelId = Guid.Parse("87654321-4321-4321-4321-cba987654321");
        ModelsController.CreateUpdateModelRequest request = new ModelsController.CreateUpdateModelRequest
        {
            ModelName = "Updated Model",
            VehicleType = "Truck",
            SeatsCount = 3,
            MaxLoadingWeightKg = 2000.0,
            EnginePowerKW = 200.0,
            TransmissionType = "Automatic",
            FuelSystemType = "Diesel",
            FuelTankVolumeLiters = "80"
        };

        _mockUpdateHandler.Handle(Arg.Any<UpdateModelCommand>()).Returns(Result.Fail("Validation error"));

        // Act
        IActionResult result = await _controller.PutModel(modelId, request);

        // Assert
        BadRequestResult? badRequestResult = result as BadRequestResult;
        Assert.NotNull(badRequestResult);
        Assert.Equal(400, badRequestResult.StatusCode);
    }

    [Fact]
    public async Task PutModel_NotFoundError_Returns404NotFound()
    {
        // Arrange
        Guid modelId = Guid.Parse("abcd1234-5678-9012-3456-789012345678");
        ModelsController.CreateUpdateModelRequest request = new ModelsController.CreateUpdateModelRequest
        {
            ModelName = "Updated Model",
            VehicleType = "Truck",
            SeatsCount = 3,
            MaxLoadingWeightKg = 2000.0,
            EnginePowerKW = 200.0,
            TransmissionType = "Automatic",
            FuelSystemType = "Diesel",
            FuelTankVolumeLiters = "80"
        };

        _mockUpdateHandler.Handle(Arg.Any<UpdateModelCommand>()).Returns(Result.Fail(new CarPark.Errors.WebApiError(404, "Model not found.")));

        // Act
        IActionResult result = await _controller.PutModel(modelId, request);

        // Assert
        NotFoundResult? notFoundResult = result as NotFoundResult;
        Assert.NotNull(notFoundResult);
        Assert.Equal(404, notFoundResult.StatusCode);
    }

    [Fact]
    public async Task DeleteModel_NotFoundError_Returns404NotFound()
    {
        // Arrange
        Guid modelId = Guid.Parse("e519ae0e-29df-4cb0-81e6-0c2979c185a9");

        _mockDeleteHandler.Handle(Arg.Any<DeleteModelCommand>()).Returns(Result.Fail(new CarPark.Errors.WebApiError(404, "Model not found.")));

        // Act
        IActionResult result = await _controller.DeleteModel(modelId);

        // Assert
        NotFoundResult? notFoundResult = result as NotFoundResult;
        Assert.NotNull(notFoundResult);
        Assert.Equal(404, notFoundResult.StatusCode);
    }

    [Fact]
    public async Task DeleteModel_ConflictError_Returns409Conflict()
    {
        // Arrange
        Guid modelId = Guid.Parse("a218ad02-9b37-4814-bbe1-5bef40d0b898");

        _mockDeleteHandler.Handle(Arg.Any<DeleteModelCommand>()).Returns(Result.Fail(new CarPark.Errors.WebApiError(409, "Cannot delete model because it is referenced by vehicles.")));

        // Act
        IActionResult result = await _controller.DeleteModel(modelId);

        // Assert
        ConflictResult? conflictResult = result as ConflictResult;
        Assert.NotNull(conflictResult);
        Assert.Equal(409, conflictResult.StatusCode);
    }

    [Fact]
    public async Task DeleteModel_HandlerFails_Returns400BadRequest()
    {
        // Arrange
        Guid modelId = Guid.Parse("72f3195d-8c1a-4c04-8277-99b0c3262c71");

        _mockDeleteHandler.Handle(Arg.Any<DeleteModelCommand>()).Returns(Result.Fail("CreateModelRequest error"));

        // Act
        IActionResult result = await _controller.DeleteModel(modelId);

        // Assert
        BadRequestResult? badRequestResult = result as BadRequestResult;
        Assert.NotNull(badRequestResult);
        Assert.Equal(400, badRequestResult.StatusCode);
    }
}