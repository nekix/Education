using Microsoft.AspNetCore.Mvc;
using NSubstitute;
using CarPark.Controllers.Api.Controllers;
using CarPark.Data;
using CarPark.Models.Vehicles;
using CarPark.Shared.CQ;
using FluentResults;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using CarPark.Identity;
using Microsoft.AspNetCore.Http;

namespace CarPark.Web.Tests.Controllers;

public class VehiclesController_Tests
{
    private readonly IVehiclesDbSet _mockSet;
    private readonly IEnterprisesDbSet _mockEnterprisesSet;
    private readonly ICommandHandler<CreateVehicleCommand, Result<int>> _mockCreateHandler;
    private readonly ICommandHandler<DeleteVehicleCommand, Result> _mockDeleteHandler;
    private readonly ICommandHandler<UpdateVehicleCommand, Result<int>> _mockUpdateHandler;
    private readonly VehiclesController _controller;

    public VehiclesController_Tests()
    {
        _mockSet = Substitute.For<IVehiclesDbSet>();
        _mockEnterprisesSet = Substitute.For<IEnterprisesDbSet>();
        _mockCreateHandler = Substitute.For<ICommandHandler<CreateVehicleCommand, Result<int>>>();
        _mockDeleteHandler = Substitute.For<ICommandHandler<DeleteVehicleCommand, Result>>();
        _mockUpdateHandler = Substitute.For<ICommandHandler<UpdateVehicleCommand, Result<int>>>();
        
        _controller = new VehiclesController(_mockSet, _mockEnterprisesSet, _mockCreateHandler, _mockUpdateHandler, _mockDeleteHandler);
        
        // Настраиваем авторизацию для тестов
        SetupAuthorization();
    }

    private void SetupAuthorization()
    {
        // Создаем claims для менеджера с ID = 1
        var claims = new List<Claim>
        {
            new Claim(AppIdentityConst.ManagerIdClaim, "1")
        };

        var identity = new ClaimsIdentity(claims, "Test");
        var principal = new ClaimsPrincipal(identity);

        // Устанавливаем User для контроллера
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = principal
            }
        };
    }

    [Fact]
    public async Task PostVehicle_ValidRequest_Returns201Created()
    {
        // Arrange
        VehiclesController.CreateUpdateVehicleRequest request = new VehiclesController.CreateUpdateVehicleRequest
        {
            ModelId = 1,
            EnterpriseId = 1,
            VinNumber = "VIN123456789",
            Price = 25000.0m,
            ManufactureYear = 2020,
            Mileage = 50000,
            Color = "Красный",
            DriversAssignments = new VehiclesController.CreateUpdateVehicleRequest.DriversAssignmentsViewModel
            {
                DriversIds = new List<int> { 1, 2 },
                ActiveDriverId = 1
            }
        };

        CreateVehicleCommand expectedCommand = new CreateVehicleCommand
        {
            RequestingManagerId = 1,
            ModelId = request.ModelId,
            EnterpriseId = request.EnterpriseId,
            VinNumber = request.VinNumber,
            Price = request.Price,
            ManufactureYear = request.ManufactureYear,
            Mileage = request.Mileage,
            Color = request.Color,
            DriverIds = request.DriversAssignments.DriversIds,
            ActiveDriverId = request.DriversAssignments.ActiveDriverId
        };

        _mockCreateHandler.Handle(Arg.Any<CreateVehicleCommand>()).Returns(Result.Ok(1));

        // Act
        ActionResult result = await _controller.PostVehicle(request);

        // Assert
        CreatedAtActionResult? createdAtResult = result as CreatedAtActionResult;
        Assert.NotNull(createdAtResult);
        Assert.Equal(201, createdAtResult.StatusCode);
        Assert.Equal("GetVehicle", createdAtResult.ActionName);
        Assert.Equal(1, createdAtResult.RouteValues?["id"]);
        
        await _mockCreateHandler.Received(1).Handle(Arg.Is<CreateVehicleCommand>(cmd => 
            cmd.ModelId == expectedCommand.ModelId &&
            cmd.EnterpriseId == expectedCommand.EnterpriseId &&
            cmd.VinNumber == expectedCommand.VinNumber &&
            cmd.Price == expectedCommand.Price &&
            cmd.ManufactureYear == expectedCommand.ManufactureYear &&
            cmd.Mileage == expectedCommand.Mileage &&
            cmd.Color == expectedCommand.Color &&
            cmd.DriverIds.SequenceEqual(expectedCommand.DriverIds) &&
            cmd.ActiveDriverId == expectedCommand.ActiveDriverId));
    }

    [Fact]
    public async Task PutVehicle_ValidRequest_Returns204NoContent()
    {
        // Arrange
        int vehicleId = 1;
        VehiclesController.CreateUpdateVehicleRequest request = new VehiclesController.CreateUpdateVehicleRequest
        {
            ModelId = 2,
            EnterpriseId = 2,
            VinNumber = "VIN987654321",
            Price = 30000.0m,
            ManufactureYear = 2021,
            Mileage = 30000,
            Color = "Синий",
            DriversAssignments = new VehiclesController.CreateUpdateVehicleRequest.DriversAssignmentsViewModel
            {
                DriversIds = new List<int> { 3, 4 },
                ActiveDriverId = 3
            }
        };

        UpdateVehicleCommand expectedCommand = new UpdateVehicleCommand
        {
            RequestingManagerId = 1,
            Id = vehicleId,
            ModelId = request.ModelId,
            EnterpriseId = request.EnterpriseId,
            VinNumber = request.VinNumber,
            Price = request.Price,
            ManufactureYear = request.ManufactureYear,
            Mileage = request.Mileage,
            Color = request.Color,
            DriverIds = request.DriversAssignments.DriversIds,
            ActiveDriverId = request.DriversAssignments.ActiveDriverId
        };

        _mockUpdateHandler.Handle(Arg.Any<UpdateVehicleCommand>()).Returns(Result.Ok(vehicleId));

        // Act
        IActionResult result = await _controller.PutVehicle(vehicleId, request);

        // Assert
        NoContentResult? noContentResult = result as NoContentResult;
        Assert.NotNull(noContentResult);
        Assert.Equal(204, noContentResult.StatusCode);
        
        await _mockUpdateHandler.Received(1).Handle(Arg.Is<UpdateVehicleCommand>(cmd => 
            cmd.Id == expectedCommand.Id &&
            cmd.ModelId == expectedCommand.ModelId &&
            cmd.EnterpriseId == expectedCommand.EnterpriseId &&
            cmd.VinNumber == expectedCommand.VinNumber &&
            cmd.Price == expectedCommand.Price &&
            cmd.ManufactureYear == expectedCommand.ManufactureYear &&
            cmd.Mileage == expectedCommand.Mileage &&
            cmd.Color == expectedCommand.Color &&
            cmd.DriverIds.SequenceEqual(expectedCommand.DriverIds) &&
            cmd.ActiveDriverId == expectedCommand.ActiveDriverId));
    }

    [Fact]
    public async Task DeleteVehicle_ValidRequest_Returns204NoContent()
    {
        // Arrange
        int vehicleId = 1;
        DeleteVehicleCommand expectedCommand = new DeleteVehicleCommand { Id = vehicleId, RequestingManagerId = 1};

        _mockDeleteHandler.Handle(Arg.Any<DeleteVehicleCommand>()).Returns(Result.Ok());

        // Act
        IActionResult result = await _controller.DeleteVehicle(vehicleId);

        // Assert
        NoContentResult? noContentResult = result as NoContentResult;
        Assert.NotNull(noContentResult);
        Assert.Equal(204, noContentResult.StatusCode);
        
        await _mockDeleteHandler.Received(1).Handle(Arg.Is<DeleteVehicleCommand>(cmd => cmd.Id == expectedCommand.Id));
    }

    [Fact]
    public async Task PostVehicle_HandlerFails_Returns400BadRequest()
    {
        // Arrange
        VehiclesController.CreateUpdateVehicleRequest request = new VehiclesController.CreateUpdateVehicleRequest
        {
            ModelId = 1,
            EnterpriseId = 1,
            VinNumber = "VIN123456789",
            Price = 25000.0m,
            ManufactureYear = 2020,
            Mileage = 50000,
            Color = "Красный",
            DriversAssignments = new VehiclesController.CreateUpdateVehicleRequest.DriversAssignmentsViewModel
            {
                DriversIds = new List<int> { 1, 2 },
                ActiveDriverId = 1
            }
        };

        _mockCreateHandler.Handle(Arg.Any<CreateVehicleCommand>()).Returns(Result.Fail("Validation error"));

        // Act
        ActionResult result = await _controller.PostVehicle(request);

        // Assert
        BadRequestResult? badRequestResult = result as BadRequestResult;
        Assert.NotNull(badRequestResult);
        Assert.Equal(400, badRequestResult.StatusCode);
    }

    [Fact]
    public async Task PutVehicle_HandlerFails_Returns400BadRequest()
    {
        // Arrange
        int vehicleId = 1;
        VehiclesController.CreateUpdateVehicleRequest request = new VehiclesController.CreateUpdateVehicleRequest
        {
            ModelId = 2,
            EnterpriseId = 2,
            VinNumber = "VIN987654321",
            Price = 30000.0m,
            ManufactureYear = 2021,
            Mileage = 30000,
            Color = "Синий",
            DriversAssignments = new VehiclesController.CreateUpdateVehicleRequest.DriversAssignmentsViewModel
            {
                DriversIds = new List<int> { 3, 4 },
                ActiveDriverId = 3
            }
        };

        _mockUpdateHandler.Handle(Arg.Any<UpdateVehicleCommand>()).Returns(Result.Fail("Validation error"));

        // Act
        IActionResult result = await _controller.PutVehicle(vehicleId, request);

        // Assert
        BadRequestResult? badRequestResult = result as BadRequestResult;
        Assert.NotNull(badRequestResult);
        Assert.Equal(400, badRequestResult.StatusCode);
    }

    [Fact]
    public async Task PutVehicle_NotFoundError_Returns404NotFound()
    {
        // Arrange
        int vehicleId = 1;
        VehiclesController.CreateUpdateVehicleRequest request = new VehiclesController.CreateUpdateVehicleRequest
        {
            ModelId = 2,
            EnterpriseId = 2,
            VinNumber = "VIN987654321",
            Price = 30000.0m,
            ManufactureYear = 2021,
            Mileage = 30000,
            Color = "Синий",
            DriversAssignments = new VehiclesController.CreateUpdateVehicleRequest.DriversAssignmentsViewModel
            {
                DriversIds = new List<int> { 3, 4 },
                ActiveDriverId = 3
            }
        };

        _mockUpdateHandler.Handle(Arg.Any<UpdateVehicleCommand>()).Returns(Result.Fail(UpdateVehicleCommand.Errors.NotFound));

        // Act
        IActionResult result = await _controller.PutVehicle(vehicleId, request);

        // Assert
        NotFoundResult? notFoundResult = result as NotFoundResult;
        Assert.NotNull(notFoundResult);
        Assert.Equal(404, notFoundResult.StatusCode);
    }

    [Fact]
    public async Task DeleteVehicle_NotFoundError_Returns404NotFound()
    {
        // Arrange
        int vehicleId = 1;

        _mockDeleteHandler.Handle(Arg.Any<DeleteVehicleCommand>()).Returns(Result.Fail(DeleteVehicleCommand.Errors.NotFound));

        // Act
        IActionResult result = await _controller.DeleteVehicle(vehicleId);

        // Assert
        NotFoundResult? notFoundResult = result as NotFoundResult;
        Assert.NotNull(notFoundResult);
        Assert.Equal(404, notFoundResult.StatusCode);
    }

    [Fact]
    public async Task DeleteVehicle_ConflictError_Returns409Conflict()
    {
        // Arrange
        int vehicleId = 1;

        _mockDeleteHandler.Handle(Arg.Any<DeleteVehicleCommand>()).Returns(Result.Fail(DeleteVehicleCommand.Errors.Conflict));

        // Act
        IActionResult result = await _controller.DeleteVehicle(vehicleId);

        // Assert
        ConflictResult? conflictResult = result as ConflictResult;
        Assert.NotNull(conflictResult);
        Assert.Equal(409, conflictResult.StatusCode);
    }

    [Fact]
    public async Task DeleteVehicle_HandlerFails_Returns400BadRequest()
    {
        // Arrange
        int vehicleId = 1;

        _mockDeleteHandler.Handle(Arg.Any<DeleteVehicleCommand>()).Returns(Result.Fail("Unknown error"));

        // Act
        IActionResult result = await _controller.DeleteVehicle(vehicleId);

        // Assert
        BadRequestResult? badRequestResult = result as BadRequestResult;
        Assert.NotNull(badRequestResult);
        Assert.Equal(400, badRequestResult.StatusCode);
    }
} 