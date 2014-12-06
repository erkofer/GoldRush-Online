using System;
using Caroline.Persistence.Models;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Caroline.Persistence
{
    public interface IUnitOfWork : IDisposable
    {
        IRepository<Game> Games { get; }
        IRepository<ApplicationUser> Users { get; }
        IRepository<IdentityRole> Roles { get; }
    }
}