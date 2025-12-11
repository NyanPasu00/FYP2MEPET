//using Facebook.Unity;
//using System;
//using System.Collections.Generic;
//using System.Threading.Tasks;
//using Unity.Services.Authentication;
//using Unity.Services.Core;
//using UnityEngine;
//using UnityEngine.SceneManagement;

//public class LoginManager : MonoBehaviour
//{
//    public LoginUI loginUI;
//    public GameUI gameUI;

//    private bool _ugsInitialized = false;
//    private bool _isSigningIn = false;

//    // Injected service for testing
//    private IAuthService authService;

//    // For normal game run, authService will be UnityAuthService unless injected
//    public void InjectAuthService(IAuthService service)
//    {
//        authService = service;
//    }

//    private async void Start()
//    {
//        // If test did NOT inject a service → use real UnityAuthService
//        if (authService == null)
//            authService = new UnityAuthService();

//        await InitializeUnityServices();
//        InitializeFacebook();
//    }

//    // ============================
//    // UNITY SERVICES INITIALIZATION
//    // ============================
//    private async Task InitializeUnityServices()
//    {
//        try
//        {
//            await authService.InitializeAsync();
//            Debug.Log("Unity Services initialized through authService");
//            _ugsInitialized = true;
//        }
//        catch (Exception ex)
//        {
//            Debug.LogError("Unity Services initialization failed: " + ex);
//        }
//    }

//    // =====================
//    // FACEBOOK INITIALIZATION
//    // =====================
//    private void InitializeFacebook()
//    {
//        if (!FB.IsInitialized)
//            FB.Init(InitCallback, OnHideUnity);
//        else
//            FB.ActivateApp();
//    }

//    private void InitCallback()
//    {
//        if (FB.IsInitialized)
//            FB.ActivateApp();
//        else
//            Debug.LogError("Failed to initialize Facebook SDK");
//    }

//    private void OnHideUnity(bool isGameShown)
//    {
//        Time.timeScale = isGameShown ? 1 : 0;
//    }

//    // =====================
//    // LOGIN LOGIC
//    // =====================
//    public async Task CheckLoginAsync()
//    {
//        Debug.Log("Checking login...");

//        if (!_ugsInitialized)
//            await InitializeUnityServices();

//        bool isSignedIn = await authService.IsSignedIn();

//        if (!isSignedIn)
//        {
//            Debug.Log("Not logged in → show login UI");
//            loginUI.displayLoginPage();
//            return;
//        }

//        Debug.Log("Logged in → checking cloud save...");

//        string json = await DataManager.LoadPetDataFromCloud();
//        Debug.Log("Cloud data: " + json);

//        PetStatus.PetData data = JsonUtility.FromJson<PetStatus.PetData>(json);

//        DigitalAlbumManager album = FindFirstObjectByType<DigitalAlbumManager>();
//        if (album != null && data != null)
//        {
//            album.LoadAlbumFromCloud(data);
//        }

//        if (string.IsNullOrEmpty(json))
//        {
//            Debug.Log("New User → Go to PetNameScene");
//            gameUI.displayNewGame();
//        }
//        else if (data.firstTime)
//        {
//            Debug.Log("Pet died → Go to PetNameScene");
//            gameUI.displayNewGame();
//        }
//        else
//        {
//            Debug.Log("Existing User → Load local data");
//            DataManager.savePetDataToLocal(json);
//            gameUI.displayGameplay();
//        }
//    }

//    // =====================
//    // FACEBOOK SIGN-IN
//    // =====================
//    public void StartFacebookSignIn()
//    {
//        if (_isSigningIn) return;
//        if (!_ugsInitialized)
//        {
//            Debug.LogError("Unity Services not initialized yet!");
//            return;
//        }

//        _isSigningIn = true;

//        var perms = new List<string>() { "public_profile", "email" };

//        FB.LogInWithReadPermissions(perms, async result =>
//        {
//            try
//            {
//                if (!FB.IsLoggedIn)
//                {
//                    Debug.Log("User cancelled Facebook login");
//                    return;
//                }

//                string token = AccessToken.CurrentAccessToken.TokenString;
//                bool isSignedIn = await authService.IsSignedIn();

//                if (!isSignedIn)
//                {
//                    await SignInWithFacebookAsync(token);
//                }
//                else if (!IsLoggedInWithFacebook())
//                {
//                    await LinkWithFacebookAsync(token);
//                }
//                else
//                {
//                    Debug.Log("Already signed in with Facebook");
//                    await FinishLoginAndReload();
//                }
//            }
//            catch (Exception ex)
//            {
//                Debug.LogException(ex);
//            }
//            finally
//            {
//                _isSigningIn = false;
//            }
//        });
//    }

//    private bool IsLoggedInWithFacebook()
//    {
//        var identities = AuthenticationService.Instance.PlayerInfo?.Identities;
//        return identities != null && identities.Exists(id => id.TypeId == "facebook");
//    }

//    private async Task SignInWithFacebookAsync(string accessToken)
//    {
//        try
//        {
//            await AuthenticationService.Instance.SignInWithFacebookAsync(accessToken);
//            loginUI.displayLoginMessage("Success Sign In with Facebook", true);
//            Debug.Log("Signed in with Facebook");
//            await FinishLoginAndReload();
//        }
//        catch (Exception ex)
//        {
//            loginUI.displayLoginMessage("Failed to Login. Please try again.", false);
//            Debug.LogError("Facebook Sign In Failed: " + ex.Message);
//        }
//    }

//    private async Task LinkWithFacebookAsync(string accessToken)
//    {
//        try
//        {
//            await AuthenticationService.Instance.LinkWithFacebookAsync(accessToken);
//            Debug.Log("Facebook account linked");
//            await FinishLoginAndReload();
//        }
//        catch (AuthenticationException ex) when (ex.ErrorCode == AuthenticationErrorCodes.AccountAlreadyLinked)
//        {
//            Debug.LogWarning("Facebook already linked");
//            await FinishLoginAndReload();
//        }
//        catch (Exception ex)
//        {
//            Debug.LogException(ex);
//        }
//    }

//    private async Task FinishLoginAndReload()
//    {
//        Debug.Log("Login complete → Returning to StartScene for cloud check");
//        await Task.Delay(1000);
//        SceneManager.LoadScene("StartScene");
//    }
//}
