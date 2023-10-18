using Microsoft.EntityFrameworkCore;
using PaintyTestApi.Auth;
using PaintyTestApi.Data;
using PaintyTestApi.Models;
using PaintyTestApi.ViewModels.AuthViewModels;

namespace PaintyTestApi.Services;

public interface IAuthService
{
    Task<AuthReqViewModel> RegAsync(LoginViewModel model);
    Task<AuthReqViewModel?> AuthAsync(LoginViewModel model);
    Task<bool> CheckLoginAsync(string login);
}

public class AuthService: IAuthService
{
    private readonly AppDbContext _db;

    public AuthService(AppDbContext db)
    {
        _db = db;
    }

    public async Task<AuthReqViewModel> RegAsync(LoginViewModel model)
    {
        var user = new User
        {
            Id = Guid.NewGuid(),
            Login = model.Login,
            Password = Password.HaspPassword(model.Password),
            Images = ""
        };

        await _db.Users.AddAsync(user);

        await _db.SaveChangesAsync();

        return new AuthReqViewModel
        {
            Id = user.Id.ToString(),
            Login = model.Login,
        };
    }

    public async Task<AuthReqViewModel?> AuthAsync(LoginViewModel model)
    {
        var user = await _db.Users
            .FirstOrDefaultAsync(user => user.Login == model.Login && user.Password == Password.HaspPassword(model.Password));

        if (user == null) return null;

        return new AuthReqViewModel
        {
            Id = user.Id.ToString(),
            Login = user.Login,
        };
    }

    public async Task<bool> CheckLoginAsync(string login)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Login == login);

        return user != null;
    }
}