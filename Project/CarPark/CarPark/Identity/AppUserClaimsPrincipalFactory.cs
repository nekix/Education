using CarPark.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using System.Security.Claims;
using CarPark.Managers;

namespace CarPark.Identity;

public class AppUserClaimsPrincipalFactory : UserClaimsPrincipalFactory<IdentityUser>
{
    private readonly ApplicationDbContext _context;

    public AppUserClaimsPrincipalFactory(
        UserManager<IdentityUser> userManager,
        IOptions<IdentityOptions> options,
        ApplicationDbContext context) 
        : base(userManager, options)
    {
        _context = context;
    }

    protected override async Task<ClaimsIdentity> GenerateClaimsAsync(IdentityUser user)
    {
        ClaimsIdentity identity = await base.GenerateClaimsAsync(user);

        Manager? manager = await _context.Managers.FirstOrDefaultAsync(m => m.IdentityUserId == user.Id);
        
        if (manager != null && !identity.HasClaim(c => c.Type == AppIdentityConst.ManagerIdClaim))
        {
            identity.AddClaim(new Claim(AppIdentityConst.ManagerIdClaim, manager.Id.ToString()));
        }

        return identity;
    }
}