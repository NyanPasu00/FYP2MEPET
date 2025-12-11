using System.Threading.Tasks;

public class FakeSlowAuthService : IAuthService
{
    public async Task InitializeAsync()
    {
        await Task.Delay(1500); // slow init
    }

    public Task<bool> IsSignedIn()
    {
        return Task.FromResult(false);
    }

    public async Task LoginAsync()
    {
        await Task.Delay(5000); // 5-second login
    }
}
