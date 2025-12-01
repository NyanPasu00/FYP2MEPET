using UnityEngine;
using UnityEngine.Networking;
public class Internet : MonoBehaviour
{

    public bool isConnect()
    {
        return Application.internetReachability != NetworkReachability.NotReachable;
    }

    public bool isValidateConnectionStatus()
    {
        // Here we just return true. You can add ping test later if needed.
        return isConnect();
    }


}
