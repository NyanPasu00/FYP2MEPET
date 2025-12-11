using System.Threading.Tasks;

public interface IAuthService
{
    Task InitializeAsync();
    Task<bool> IsSignedIn();
    Task LoginAsync();
}