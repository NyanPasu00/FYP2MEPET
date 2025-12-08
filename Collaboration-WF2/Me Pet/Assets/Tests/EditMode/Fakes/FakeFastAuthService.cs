using System.Threading.Tasks;

public class FakeFastAuthService : IAuthService
{
    public async Task InitializeAsync()
    {
        await Task.Delay(50); // fast init simulation
    }

    public Task<bool> IsSignedIn()
    {
        return Task.FromResult(true);
    }

    public async Task LoginAsync()
    {
        await Task.Delay(100); // simulating fast login
    }
}
