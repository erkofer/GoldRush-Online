using System;
using Caroline.Models;
using Microsoft.AspNet.Identity.EntityFramework;

namespace Caroline.Dal
{
    public class UnitOfWork : IDisposable
    {
        ApplicationDbContext context = ApplicationDbContext.Create();
        Repository<Game> _games;
        Repository<ApplicationUser> _users;
        Repository<IdentityRole> _roles; 

        public void Dispose()
        {
            _games = new Repository<Game>(context);
            _users = new Repository<ApplicationUser>(context);
            _roles = new Repository<IdentityRole>(context);
        }
    }
}