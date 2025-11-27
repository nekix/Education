using CarPark.Identity;
using CarPark.ManagersOperations.Rides.Queries;
using CarPark.ManagersOperations.Tracks.Commands;
using CarPark.ManagersOperations.Tracks.Queries;
using CarPark.ManagersOperations.Tracks.Queries.Models;
using CarPark.ManagersOperations.Vehicles;
using CarPark.ManagersOperations.Vehicles.Commands;
using CarPark.ManagersOperations.Vehicles.Queries;
using CarPark.ManagersOperations.Vehicles.Queries.Models;
using CarPark.Shared.CQ;
using CarPark.Shared.CQ;
using FluentResults;
using FluentResults;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using NetTopologySuite.Features;
using NSubstitute;
using System.Security.Claims;
using VehiclesController = CarPark.Controllers.Api.Controllers.VehiclesController;

namespace CarPark.Web.Tests.Controllers;

public class VehiclesController_Tests
{
    private readonly IQueryHandler<GetVehicleQuery, Result<VehicleDto>> _mockGetVehicleHandler;
    private readonly IQueryHandler<GetVehiclesListQuery, Result<PaginatedVehicles>> _mockGetVehiclesListHandler;
    private readonly IQueryHandler<GetTrackQuery, Result<TrackViewModel>> _mockGetTrackHandler;
    private readonly IQueryHandler<GetTrackFeatureCollectionQuery, Result<FeatureCollection>> _mockGetTrackFeatureCollectionHandler;
    private readonly IQueryHandler<GetRidesTrackQuery, Result<TrackViewModel>> _mockGetRidesTrackHandler;
    private readonly IQueryHandler<GetRidesTrackFeatureCollectionQuery, Result<FeatureCollection>> _mockGetRidesTrackFeatureCollectionHandler;
    private readonly ICommandHandler<CreateVehicleCommand, Result<Guid>> _mockCreateHandler;
    private readonly ICommandHandler<DeleteVehicleCommand, Result> _mockDeleteHandler;
    private readonly ICommandHandler<UpdateVehicleCommand, Result<Guid>> _mockUpdateHandler;
    private readonly IQueryHandler<GetRidesQuery, Result<RidesViewModel>> _mockGetRidesQueryHandler;
    private readonly ICommandHandler<CreateRideFromGpxFileCommand, Result<Guid>> _mockCreateRideFromGpxFileCommandHandler;
    private readonly VehiclesController _controller;

    public VehiclesController_Tests()
    {
        _mockGetVehicleHandler = Substitute.For<IQueryHandler<GetVehicleQuery, Result<VehicleDto>>>();
        _mockGetVehiclesListHandler = Substitute.For<IQueryHandler<GetVehiclesListQuery, Result<PaginatedVehicles>>>();
        _mockGetTrackHandler = Substitute.For<IQueryHandler<GetTrackQuery, Result<TrackViewModel>>>();
        _mockGetTrackFeatureCollectionHandler = Substitute.For<IQueryHandler<GetTrackFeatureCollectionQuery, Result<FeatureCollection>>>();
        _mockGetRidesTrackHandler = Substitute.For<IQueryHandler<GetRidesTrackQuery, Result<TrackViewModel>>>();
        _mockGetRidesTrackFeatureCollectionHandler = Substitute.For<IQueryHandler<GetRidesTrackFeatureCollectionQuery, Result<FeatureCollection>>>();
        _mockCreateHandler = Substitute.For<ICommandHandler<CreateVehicleCommand, Result<Guid>>>();
        _mockDeleteHandler = Substitute.For<ICommandHandler<DeleteVehicleCommand, Result>>();
        _mockUpdateHandler = Substitute.For<ICommandHandler<UpdateVehicleCommand, Result<Guid>>>();
        _mockGetRidesQueryHandler = Substitute.For<IQueryHandler<GetRidesQuery, Result<RidesViewModel>>>();
        _mockCreateRideFromGpxFileCommandHandler =
            Substitute.For<ICommandHandler<CreateRideFromGpxFileCommand, Result<Guid>>>();

        _controller = new VehiclesController(
            _mockGetVehicleHandler,
            _mockGetVehiclesListHandler,
            _mockCreateHandler,
            _mockUpdateHandler,
            _mockDeleteHandler,
            _mockCreateRideFromGpxFileCommandHandler,
            _mockGetTrackHandler,
            _mockGetTrackFeatureCollectionHandler,
            _mockGetRidesTrackHandler,
            _mockGetRidesTrackFeatureCollectionHandler,
            _mockGetRidesQueryHandler);

        // Настраиваем авторизацию для тестов
        SetupAuthorization();
    }

    private void SetupAuthorization()
    {
        // Создаем claims для менеджера с ID = 1
        var claims = new List<Claim>
        {
            new Claim(AppIdentityConst.ManagerIdClaim, "9a437251-d4d6-4aaa-b742-36a428619a94")
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
        Guid expectedId = Guid.Parse("72f3195d-8c1a-4c04-8277-99b0c3262c71");
        VehiclesController.CreateUpdateVehicleRequest request = new VehiclesController.CreateUpdateVehicleRequest
        {
            ModelId = Guid.Parse("9a437251-d4d6-4aaa-b742-36a428619a94"),
            EnterpriseId = Guid.Parse("5cbc7623-8636-45c4-8c01-da079c60c2ef"),
            VinNumber = "VIN123456789",
            Price = 25000.0m,
            ManufactureYear = 2020,
            Mileage = 50000,
            Color = "Красный",
            AddedToEnterpriseAt = DateTimeOffset.Parse("2025.03.12"),
            DriversAssignments = new VehiclesController.CreateUpdateVehicleRequest.DriversAssignmentsViewModel
            {
                DriversIds = new List<Guid> { Guid.Parse("9ad5d7a3-b91c-45db-aadb-c761830c4d9c"), Guid.Parse("10df230b-809c-46c3-a3ca-0a67a923f6ab") },
                ActiveDriverId = Guid.Parse("9ad5d7a3-b91c-45db-aadb-c761830c4d9c")
            }
        };

        CreateVehicleCommand expectedCommand = new CreateVehicleCommand
        {
            RequestingManagerId = Guid.Parse("9a437251-d4d6-4aaa-b742-36a428619a94"),
            ModelId = request.ModelId,
            EnterpriseId = request.EnterpriseId,
            VinNumber = request.VinNumber,
            Price = request.Price,
            ManufactureYear = request.ManufactureYear,
            Mileage = request.Mileage,
            Color = request.Color,
            AddedToEnterpriseAt = request.AddedToEnterpriseAt,
            DriverIds = request.DriversAssignments.DriversIds,
            ActiveDriverId = request.DriversAssignments.ActiveDriverId
        };

        _mockCreateHandler.Handle(Arg.Any<CreateVehicleCommand>()).Returns(Result.Ok(expectedId));

        // Act
        ActionResult result = await _controller.PostVehicle(request);

        // Assert
        CreatedAtActionResult? createdAtResult = result as CreatedAtActionResult;
        Assert.NotNull(createdAtResult);
        Assert.Equal(201, createdAtResult.StatusCode);
        Assert.Equal("GetVehicle", createdAtResult.ActionName);
        Assert.Equal(expectedId, createdAtResult.RouteValues?["id"]);
        
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
        Guid vehicleId = Guid.Parse("3fb09dc3-f8ae-433f-b647-0afc2682190e");
        VehiclesController.CreateUpdateVehicleRequest request = new VehiclesController.CreateUpdateVehicleRequest
        {
            ModelId = Guid.Parse("8d16dea5-cb7c-4fd4-80a2-bf98d5fa9bc7"),
            EnterpriseId = Guid.Parse("286ce562-09f6-413d-86b7-4b3a60ac8163"),
            VinNumber = "VIN987654321",
            Price = 30000.0m,
            ManufactureYear = 2021,
            Mileage = 30000,
            Color = "Синий",
            AddedToEnterpriseAt = DateTimeOffset.Parse("2025.03.12"),
            DriversAssignments = new VehiclesController.CreateUpdateVehicleRequest.DriversAssignmentsViewModel
            {
                DriversIds = new List<Guid> { Guid.Parse("ac9db35d-ec8b-4343-92a7-3834487199cc"), Guid.Parse("0e837b72-38ed-4114-b8e3-9510b910cd08") },
                ActiveDriverId = Guid.Parse("ac9db35d-ec8b-4343-92a7-3834487199cc")
            }
        };

        UpdateVehicleCommand expectedCommand = new UpdateVehicleCommand
        {
            RequestingManagerId = Guid.Parse("9a437251-d4d6-4aaa-b742-36a428619a94"),
            VehicleId = vehicleId,
            ModelId = request.ModelId,
            EnterpriseId = request.EnterpriseId,
            VinNumber = request.VinNumber,
            Price = request.Price,
            ManufactureYear = request.ManufactureYear,
            Mileage = request.Mileage,
            Color = request.Color,
            AddedToEnterpriseAt = request.AddedToEnterpriseAt,
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
            cmd.VehicleId == expectedCommand.VehicleId &&
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
        Guid vehicleId = Guid.Parse("5d9c2ae9-fc5e-45c7-a9a5-991e8cb5ecc2");
        DeleteVehicleCommand expectedCommand = new DeleteVehicleCommand { VehicleId = vehicleId, RequestingManagerId = Guid.Parse("9a437251-d4d6-4aaa-b742-36a428619a94")};

        _mockDeleteHandler.Handle(Arg.Any<DeleteVehicleCommand>()).Returns(Result.Ok());

        // Act
        IActionResult result = await _controller.DeleteVehicle(vehicleId);

        // Assert
        NoContentResult? noContentResult = result as NoContentResult;
        Assert.NotNull(noContentResult);
        Assert.Equal(204, noContentResult.StatusCode);
        
        await _mockDeleteHandler.Received(1).Handle(Arg.Is<DeleteVehicleCommand>(cmd => cmd.VehicleId == expectedCommand.VehicleId));
    }

    [Fact]
    public async Task PostVehicle_HandlerFails_Returns400BadRequest()
    {
        // Arrange
        VehiclesController.CreateUpdateVehicleRequest request = new VehiclesController.CreateUpdateVehicleRequest
        {
            ModelId = Guid.Parse("9a437251-d4d6-4aaa-b742-36a428619a94"),
            EnterpriseId = Guid.Parse("5cbc7623-8636-45c4-8c01-da079c60c2ef"),
            VinNumber = "VIN123456789",
            Price = 25000.0m,
            ManufactureYear = 2020,
            Mileage = 50000,
            Color = "Красный",
            AddedToEnterpriseAt = DateTimeOffset.Parse("2025.03.12"),
            DriversAssignments = new VehiclesController.CreateUpdateVehicleRequest.DriversAssignmentsViewModel
            {
                DriversIds = new List<Guid> { Guid.Parse("9ad5d7a3-b91c-45db-aadb-c761830c4d9c"), Guid.Parse("10df230b-809c-46c3-a3ca-0a67a923f6ab") },
                ActiveDriverId = Guid.Parse("9ad5d7a3-b91c-45db-aadb-c761830c4d9c")
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
        Guid vehicleId = Guid.Parse("5c61499a-6041-4cf5-befa-fa96830df655");
        VehiclesController.CreateUpdateVehicleRequest request = new VehiclesController.CreateUpdateVehicleRequest
        {
            ModelId = Guid.Parse("8d16dea5-cb7c-4fd4-80a2-bf98d5fa9bc7"),
            EnterpriseId = Guid.Parse("286ce562-09f6-413d-86b7-4b3a60ac8163"),
            VinNumber = "VIN987654321",
            Price = 30000.0m,
            ManufactureYear = 2021,
            Mileage = 30000,
            Color = "Синий",
            AddedToEnterpriseAt = DateTimeOffset.Parse("2025.03.12"),
            DriversAssignments = new VehiclesController.CreateUpdateVehicleRequest.DriversAssignmentsViewModel
            {
                DriversIds = new List<Guid> { Guid.Parse("ac9db35d-ec8b-4343-92a7-3834487199cc"), Guid.Parse("0e837b72-38ed-4114-b8e3-9510b910cd08") },
                ActiveDriverId = Guid.Parse("ac9db35d-ec8b-4343-92a7-3834487199cc")
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
        Guid vehicleId = Guid.Parse("405bcd11-cbb9-49b3-b294-7f223b4198de");
        VehiclesController.CreateUpdateVehicleRequest request = new VehiclesController.CreateUpdateVehicleRequest
        {
            ModelId = Guid.Parse("8d16dea5-cb7c-4fd4-80a2-bf98d5fa9bc7"),
            EnterpriseId = Guid.Parse("286ce562-09f6-413d-86b7-4b3a60ac8163"),
            VinNumber = "VIN987654321",
            Price = 30000.0m,
            ManufactureYear = 2021,
            Mileage = 30000,
            Color = "Синий",
            AddedToEnterpriseAt = DateTimeOffset.Parse("2025.03.12"),
            DriversAssignments = new VehiclesController.CreateUpdateVehicleRequest.DriversAssignmentsViewModel
            {
                DriversIds = new List<Guid> { Guid.Parse("ac9db35d-ec8b-4343-92a7-3834487199cc"), Guid.Parse("0e837b72-38ed-4114-b8e3-9510b910cd08") },
                ActiveDriverId = Guid.Parse("ac9db35d-ec8b-4343-92a7-3834487199cc")
            }
        };

        _mockUpdateHandler.Handle(Arg.Any<UpdateVehicleCommand>()).Returns(Result.Fail(VehiclesHandlersErrors.VehicleNotExist));

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
        Guid vehicleId = Guid.Parse("bf044397-9bc9-490b-9ddc-22f1922cbdc1");

        _mockDeleteHandler.Handle(Arg.Any<DeleteVehicleCommand>()).Returns(Result.Fail(VehiclesHandlersErrors.VehicleNotExist));

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
        Guid vehicleId = Guid.Parse("68d8c7ab-47e2-43cf-8c44-500fb7020024");

        _mockDeleteHandler.Handle(Arg.Any<DeleteVehicleCommand>()).Returns(Result.Fail(VehiclesHandlersErrors.ForbidDeleteVehicleWithAssignedDrivers));

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
        Guid vehicleId = Guid.Parse("d66c478e-ba81-44d3-82e5-0b735f229073");

        _mockDeleteHandler.Handle(Arg.Any<DeleteVehicleCommand>()).Returns(Result.Fail("CreateModelRequest error"));

        // Act
        IActionResult result = await _controller.DeleteVehicle(vehicleId);

        // Assert
        BadRequestResult? badRequestResult = result as BadRequestResult;
        Assert.NotNull(badRequestResult);
        Assert.Equal(400, badRequestResult.StatusCode);
    }
} 