using CarPark.Errors;
using CarPark.Enterprises.Errors;
using FluentResults;

namespace CarPark.ManagersOperations.Enterprises;

public static class EnterprisesErrors
{
    public static Error MapDomainError(EnterpriseDomainError domainError)
    {
        return domainError.Code switch
        {
            EnterpriseDomainErrorCodes.EnterpriseHasOtherManagersError => new WebApiError(409, "Cannot delete enterprise because it has assigned managers.").CausedBy(domainError),
            EnterpriseDomainErrorCodes.EnterpriseHasVehiclesError => new WebApiError(409, "Cannot delete enterprise because it has assigned vehicles.").CausedBy(domainError),
            EnterpriseDomainErrorCodes.EnterpriseHasDriversError => new WebApiError(409, "Cannot delete enterprise because it has assigned drivers.").CausedBy(domainError),
            _ => new WebApiError(500, "An unexpected error occurred.").CausedBy(domainError)
        };
    }
}
