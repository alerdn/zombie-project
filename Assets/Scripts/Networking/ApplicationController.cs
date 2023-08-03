using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ApplicationController : MonoBehaviour
{
    [SerializeField] private ClientSingleton _clientPrefab;
    [SerializeField] private HostSingleton _hostPrefab;
    [SerializeField] private ServerSingleton _serverPrefab;
    [SerializeField] private NetworkObject _playerPrefab;

    private ApplicationData _applicationData;

    private async void Start()
    {
        DontDestroyOnLoad(gameObject);

        await LaunchInMode(SystemInfo.graphicsDeviceType == UnityEngine.Rendering.GraphicsDeviceType.Null);
    }

    private async Task LaunchInMode(bool isDedicatedServer)
    {
        if (isDedicatedServer)
        {
            Application.targetFrameRate = 60;

            _applicationData = new ApplicationData();

            ServerSingleton serverSingleton = Instantiate(_serverPrefab);

            StartCoroutine(LoadGameSceneAsync(serverSingleton));
        }
        else
        {
            ClientSingleton clientSingleton = Instantiate(_clientPrefab);
            bool isAuthenticated = await clientSingleton.CreateClient();

            HostSingleton hostSingleton = Instantiate(_hostPrefab);
            hostSingleton.CreateHost(_playerPrefab);

            if (isAuthenticated)
            {
                clientSingleton.GameManager.GoToMenu();
            }
        }
    }

    private IEnumerator LoadGameSceneAsync(ServerSingleton serverSingleton)
    {
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(Constants.GameSceneName);

        while (!asyncOperation.isDone)
        {
            yield return null;
        }

        Task createServerTask = serverSingleton.CreateServer(_playerPrefab);
        yield return new WaitUntil(() => createServerTask.IsCompleted);

        Task startServertTask = serverSingleton.GameManager.StartGameServerAsync();
        yield return new WaitUntil(() => startServertTask.IsCompleted);
    }
}
