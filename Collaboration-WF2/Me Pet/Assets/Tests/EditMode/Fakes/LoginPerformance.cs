//using NUnit.Framework;
//using System.Diagnostics;
//using System.Threading.Tasks;
//using UnityEngine;

//public class LoginPerformanceTests
//{
//    private const int MaxAllowedMs = 3000;

//    [Test]
//    public async Task LoginCompletesWithin3Seconds()
//    {
//        // Arrange
//        var go = new GameObject();
//        var loginManager = go.AddComponent<LoginManager>();
//        var fakeAuth = new FakeFastAuthService();

//        loginManager.InjectAuthService(fakeAuth);

//        var stopwatch = Stopwatch.StartNew();

//        // Act
//        await fakeAuth.InitializeAsync();
//        await fakeAuth.LoginAsync();

//        stopwatch.Stop();
//        long ms = stopwatch.ElapsedMilliseconds;

//        // Assert
//        Assert.LessOrEqual(ms, MaxAllowedMs,
//            $"Login too slow! Took {ms} ms");
//    }

//    [Test]
//    public async Task SlowLoginFailsThreshold()
//    {
//        // Arrange
//        var go = new GameObject();
//        var loginManager = go.AddComponent<LoginManager>();
//        var fakeAuth = new FakeSlowAuthService();

//        loginManager.InjectAuthService(fakeAuth);

//        var stopwatch = Stopwatch.StartNew();

//        // Act
//        await fakeAuth.InitializeAsync();
//        await fakeAuth.LoginAsync();

//        stopwatch.Stop();
//        long ms = stopwatch.ElapsedMilliseconds;

//        // Assert (should exceed)
//        Assert.Greater(ms, MaxAllowedMs,
//            $"Slow login should exceed limit, but was {ms}ms");
//    }
//}
