using Microsoft.EntityFrameworkCore;
using PaintyTestApi.Data;

namespace PaintyTestApi.Services;

public interface IMediaService
{
    Task AddImageAsync(string userId, string fileName);
    Task<List<string>?> GetUserImagesAsync(string userId);
    Task<List<string>?> GetFriendImagesAsync(string userId, string friendId);
}

public class MediaService : IMediaService
{
    private readonly AppDbContext _db;

    public MediaService(AppDbContext db)
    {
        _db = db;
    }

    public async Task AddImageAsync(string userId, string fileName)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == Guid.Parse(userId));

        if (user == null) return;

        var images = user.Images.Split('$').ToList();
        if (images.ElementAt(0) == "") images.RemoveAt(0);
        images.Add(fileName);
        user.Images = string.Join('$', images);

        await _db.SaveChangesAsync();
    }

    public async Task<List<string>?> GetUserImagesAsync(string userId)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Id == Guid.Parse(userId));

        if (user == null) return null;

        var images = user.Images.Split('$').ToList();
        if (images.ElementAt(0) == "") images.RemoveAt(0);

        return images;
    }

    public async Task<List<string>?> GetFriendImagesAsync(string userId, string friendId)
    {
        var friendUser = await _db.Users
            .Include(u => u.Friends)
            .FirstOrDefaultAsync(u => u.Id == Guid.Parse(friendId));

        if(friendUser?.Friends.FirstOrDefault(f => f.Id == Guid.Parse(userId)) == null) return null;

        var images = friendUser.Images.Split('$').ToList();
        if (images.ElementAt(0) == "") images.RemoveAt(0);

        return images;
    }
}