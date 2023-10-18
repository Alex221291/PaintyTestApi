using Microsoft.EntityFrameworkCore;
using PaintyTestApi.Data;
using PaintyTestApi.ViewModels.UserViewModels;

namespace PaintyTestApi.Services;

public interface IUserService
{
    Task<List<UserViewModel>?> GetUsersAsync(string userId);
    Task<bool?> AddFiendAsync(string userId, string friendId);
    Task<List<UserViewModel>?> GetFiendsAsync(string userId);
}

public class UserService: IUserService
{
    private readonly AppDbContext _db;

    public UserService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<List<UserViewModel>?> GetUsersAsync(string userId)
    {
        var users = await _db.Users
            .Select(user => new UserViewModel 
            { 
                Id = user.Id.ToString(),
                Login = user.Login,
            })
            .Where(u => u.Id != userId)
            .ToListAsync();

        return users;
    }

    public async Task<bool?> AddFiendAsync(string userId, string friendId)
    {
        var user = await _db.Users
            .Include(u => u.Friends)
            .FirstOrDefaultAsync(u => u.Id == Guid.Parse(userId));
        if (user == null) return null;

        var friend = await _db.Users
            .FirstOrDefaultAsync(u => u.Id == Guid.Parse(friendId));

        if (friend == null) return null;

        var checkFriend = user.Friends.Contains(friend);

        if (checkFriend) return false;

        user.Friends.Add(friend);

        await _db.SaveChangesAsync();

        return true;
    }

    public async Task<List<UserViewModel>?> GetFiendsAsync(string userId)
    {
        var user = await _db.Users
            .Include(u => u.Friends)
            .FirstOrDefaultAsync(u => u.Id == Guid.Parse(userId));

        return user?.Friends.Select(f => new UserViewModel
        {
            Id = f.Id.ToString(),
            Login = f.Login
        }).ToList();
    }
}