using Data.Contexts;
using Data.Entities;
using Domain.Models;

namespace Data.Repositories;
public interface IAppUserRepository : IBaseRepository<AppUserEntity, AppUser>
{
}
public class AppUserRepository(DataContext context) : BaseRepository<AppUserEntity, AppUser>(context), IAppUserRepository
{
}
