using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class ApplicationController : MonoBehaviour
{
    [SerializeField] private ClientSingleton _clientPrefab;
    [SerializeField] private HostSingleton _hostPrefab;

    private async void Start()
    {
        DontDestroyOnLoad(gameObject);

        await LaunchInMode(SystemInfo.graphicsDeviceType == UnityEngine.Rendering.GraphicsDeviceType.Null);
    }

    private async Task LaunchInMode(bool isDedicatedServer)
    {
        if (isDedicatedServer)
        {

        }
        else
        {
            ClientSingleton clientSingleton = Instantiate(_clientPrefab);
            bool isAuthenticated = await clientSingleton.CreateClient();

            HostSingleton hostSingleton = Instantiate(_hostPrefab);
            hostSingleton.CreateHost();

            if (isAuthenticated)
            {
                clientSingleton.GameManager.GoToMenu();
            }
        }
    }
}
