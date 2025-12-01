using Facebook.Unity;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Core;
using UnityEngine;
using UnityEngine.SceneManagement;


public class LoginManager : MonoBehaviour
{
    public LoginUI loginUI;

    private bool _ugsInitialized = false;
    private bool _isSigningIn = false;

    private async void Start()
    {
        await InitializeUnityServices();
        InitializeFacebook();

        // After initialization, check login
        //await CheckLoginAsync();
    }

    #region Unity Services Initialization
    private async Task InitializeUnityServices()
    {
        try
        {
            if (!UnityServices.State.Equals(ServicesInitializationState.Initialized))
            {
                await UnityServices.InitializeAsync();
            }

            Debug.Log("Unity Services initialized");
            _ugsInitialized = true;

            // Anonymous login first
            //if (!AuthenticationService.Instance.IsSignedIn)
            //{
            //    await AuthenticationService.Instance.SignInAnonymouslyAsync();
            //    Debug.Log("Anonymous sign-in completed");
            //}
        }
        catch (Exception ex)
        {
            Debug.LogError("Unity Services initialization failed: " + ex);
        }
    }
    #endregion

    #region Facebook Initialization
    private void InitializeFacebook()
    {
        if (!FB.IsInitialized)
            FB.Init(InitCallback, OnHideUnity);
        else
            FB.ActivateApp();
    }

    private void InitCallback()
    {
        if (FB.IsInitialized) FB.ActivateApp();
        else Debug.LogError("Failed to initialize Facebook SDK");
    }

    private void OnHideUnity(bool isGameShown)
    {
        Time.timeScale = isGameShown ? 1 : 0;
    }
    #endregion

    #region Login Logic
    public async Task CheckLoginAsync()
    {
        Debug.Log("Checking login...");

        if (!_ugsInitialized)
            await InitializeUnityServices();

        if (!AuthenticationService.Instance.IsSignedIn)
        {
            Debug.Log("Not logged in → go to LoginScene");
            SceneManager.LoadScene("LoginScene");
            return;
        }

        // Already logged in → Check cloud data
        Debug.Log("Logged in → checking cloud save...");

        string json = await CloudSaveManager.LoadPetDataFromCloud();
        Debug.Log(json);

        PetStatus.PetData data = JsonUtility.FromJson<PetStatus.PetData>(json);
        

        DigitalAlbumManager album = FindFirstObjectByType<DigitalAlbumManager>();

        if (album != null && data != null)
        {
            album.LoadAlbumFromCloud(data);
        }

        if (string.IsNullOrEmpty(json))
        {
            Debug.Log("New User → Go to PetNameScene");
            SceneManager.LoadScene("PetNameScene");
        }
        else if (data.firstTime)
        {
            // Pet died → must create a new pet
            Debug.Log("Create New Pie → Go to PetNameScene");
            SceneManager.LoadScene("PetNameScene");
        }
        else
        {
            Debug.Log("Existing User → Loading local data");
            PlayerPrefs.SetString("PetData", json);
            PlayerPrefs.Save();

            loginUI?.OnLoginDone();
        }
    }
    #endregion

    #region Facebook Sign-In
    public void StartFacebookSignIn()
    {
        if (_isSigningIn) return;
        if (!_ugsInitialized)
        {
            Debug.LogError("Unity Services not initialized yet!");
            return;
        }

        _isSigningIn = true;

        var perms = new List<string>() { "public_profile", "email" };
        FB.LogInWithReadPermissions(perms, async result =>
        {
            try
            {
                if (!FB.IsLoggedIn)
                {
                    Debug.Log("User cancelled Facebook login");
                    return;
                }

                string token = AccessToken.CurrentAccessToken.TokenString;

                if (!AuthenticationService.Instance.IsSignedIn)
                {
                    await SignInWithFacebookAsync(token);
                }
                else if (!IsLoggedInWithFacebook())
                {
                    await LinkWithFacebookAsync(token);
                }
                else
                {
                    Debug.Log("Already signed in with Facebook");
                    await FinishLoginAndReload();
                }
            }
            catch (Exception ex)
            {
                Debug.LogException(ex);
            }
            finally
            {
                _isSigningIn = false;
            }
        });
    }

    private bool IsLoggedInWithFacebook()
    {
        var identities = AuthenticationService.Instance.PlayerInfo?.Identities;
        return identities != null && identities.Exists(id => id.TypeId == "facebook");
    }

    private async Task SignInWithFacebookAsync(string accessToken)
    {
        try
        {
            await AuthenticationService.Instance.SignInWithFacebookAsync(accessToken);
            Debug.Log("Signed in with Facebook");
            await FinishLoginAndReload();
        }
        catch (Exception ex)
        {
            Debug.LogError("Facebook Sign In Failed: " + ex.Message);
        }
    }

    private async Task LinkWithFacebookAsync(string accessToken)
    {
        try
        {
            await AuthenticationService.Instance.LinkWithFacebookAsync(accessToken);
            Debug.Log("Facebook account linked");
            await FinishLoginAndReload();
        }
        catch (AuthenticationException ex) when (ex.ErrorCode == AuthenticationErrorCodes.AccountAlreadyLinked)
        {
            Debug.LogWarning("Facebook already linked");
            await FinishLoginAndReload();
        }
        catch (Exception ex)
        {
            Debug.LogException(ex);
        }
    }

    private async Task FinishLoginAndReload()
    {
        Debug.Log("Login complete → Returning to StartScene for cloud check");
        await Task.Delay(300); // small delay
        SceneManager.LoadScene("StartScene");
    }
    #endregion
}
