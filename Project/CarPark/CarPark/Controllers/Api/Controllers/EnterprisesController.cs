﻿using CarPark.Attributes;
using CarPark.Data;
using CarPark.Identity;
using CarPark.ManagersOperations;
using CarPark.ManagersOperations.Enterprises;
using CarPark.ManagersOperations.Enterprises.Commands;
using CarPark.ManagersOperations.Enterprises.Queries;
using CarPark.Shared.CQ;
using CarPark.ViewModels.Api;
using FluentResults;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CarPark.Controllers.Api.Controllers;

[Authorize(AppIdentityConst.ManagerPolicy)]
public class EnterprisesController : ApiBaseController
{
    private readonly ApplicationDbContext _context;
    private readonly ICommandHandler<DeleteEnterpriseCommand, Result> _deleteEnterpriseHandler;

    private readonly IQueryHandler<GetEnterpriseQuery, Result<EnterpriseDto>> _getEnterpriseQueryHandler;
    private readonly IQueryHandler<GetEnterprisesCollectionQuery, Result<List<EnterpriseDto>>> _getEnterprisesCollectionQuery;

    public EnterprisesController(ApplicationDbContext context,
        ICommandHandler<DeleteEnterpriseCommand, Result> deleteEnterpriseHandler,
        IQueryHandler<GetEnterpriseQuery, Result<EnterpriseDto>> getEnterpriseQueryHandler,
        IQueryHandler<GetEnterprisesCollectionQuery, Result<List<EnterpriseDto>>> getEnterprisesCollectionQuery)
    {
        _context = context;

        _deleteEnterpriseHandler = deleteEnterpriseHandler;

        _getEnterpriseQueryHandler = getEnterpriseQueryHandler;
        _getEnterprisesCollectionQuery = getEnterprisesCollectionQuery;
    }

    // GET: api/Enterprises
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(IEnumerable<EnterpriseDto>))]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<IEnumerable<EnterpriseViewModel>>> GetEnterprises()
    {
        int managerId = GetCurrentManagerId();

        GetEnterprisesCollectionQuery query = new GetEnterprisesCollectionQuery
        {
            RequestingManagerId = managerId
        };

        Result<List<EnterpriseDto>> getEnterprisesCollection = await _getEnterprisesCollectionQuery.Handle(query);

        if (getEnterprisesCollection.IsSuccess)
        {
            return Ok(getEnterprisesCollection.Value);
        }

        if (getEnterprisesCollection.HasError(e => e.Message == ManagersOperationsErrors.ManagerNotExist))
        {
            return Forbid();
        }
        else
        {
            return BadRequest();
        }
    }

    // GET: api/Enterprises/5
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(EnterpriseDto))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<EnterpriseViewModel>> GetEnterprise(int id)
    {
        int managerId = GetCurrentManagerId();

        GetEnterpriseQuery query = new GetEnterpriseQuery()
        {
            RequestingManagerId = managerId,
            EnterpriseId = id
        };

        Result<EnterpriseDto> getEnterprise = await _getEnterpriseQueryHandler.Handle(query);

        if (getEnterprise.IsSuccess)
        {
            return Ok(getEnterprise.Value);
        }

        if (getEnterprise.HasError(e => e.Message == ManagersOperationsErrors.ManagerNotExist))
        {
            return Forbid();
        }
        else if (getEnterprise.HasError(e => e.Message == EnterprisesHandlersErrors.ManagerNotAllowedToEnterprise))
        {
            return Forbid();
        }
        else
        {
            return BadRequest();
        }
    }

    // DELETE: api/Enterprises/5
    [HttpDelete("{id}")]
    [AppValidateAntiForgeryToken]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<IActionResult> DeleteEnterprise(int id)
    {
        int managerId = GetCurrentManagerId();

        DeleteEnterpriseCommand command = new DeleteEnterpriseCommand
        {
            RequestingManagerId = managerId,
            EnterpriseId = id,
        };

        Result deleteEnterprise = await _deleteEnterpriseHandler.Handle(command);

        // Success flow
        if (deleteEnterprise.IsSuccess)
        {
            return NoContent();
        }

        // Errors handling
        if (deleteEnterprise.HasError(e => e.Message == ManagersOperationsErrors.ManagerNotExist))
        {
            return Forbid();
        }
        else if (deleteEnterprise.HasError(e => e.Message == EnterprisesHandlersErrors.EnterpriseNotExist))
        {
            return NotFound();
        }
        else if (deleteEnterprise.HasError(e => e.Message == EnterprisesHandlersErrors.ManagerNotAllowedToEnterprise))
        {
            return Forbid();
        }
        else if (deleteEnterprise.HasError(e => e.Message == EnterprisesHandlersErrors.ForbidDeleteEnterpriseWithOtherManagers))
        {
            return Conflict();
        }
        else if (deleteEnterprise.HasError(e => e.Message == EnterprisesHandlersErrors.ForbidDeleteWithAnyVehicles))
        {
            return Conflict();
        }
        else if (deleteEnterprise.HasError(e => e.Message == EnterprisesHandlersErrors.ForbidDeleteWithAnyDrivers))
        {
            return Conflict();
        }
        else
        {
            return BadRequest();
        }
    }
}