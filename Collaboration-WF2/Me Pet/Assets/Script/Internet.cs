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
        return isConnect();
    }


}
