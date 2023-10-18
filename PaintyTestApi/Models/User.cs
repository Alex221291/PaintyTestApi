namespace PaintyTestApi.Models;

public class User
{
    public Guid Id { get; set; }
    public string Login { get; set; }
    public string Password { get; set; }
    public string Images { get; set; }

    public ICollection<User> Users { get; set; } = new List<User>();
    public ICollection<User> Friends { get; set; } = new List<User>();
}