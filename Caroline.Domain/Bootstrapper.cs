using AutoMapper;
using Caroline.Domain.Models;
using Caroline.Persistence.Models;

namespace Caroline.Domain
{
    public static class StaticBootstrapper
    {
        static StaticBootstrapper()
        {
            Mapper.CreateMap<User, UserProfile>().AfterMap((u,up)=> { if (up.IsAnonymous) up.UserName = null; });
        }
    }
}
