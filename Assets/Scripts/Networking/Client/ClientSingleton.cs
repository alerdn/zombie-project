using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class ClientSingleton : Singleton<ClientSingleton>
{
    private ClientGameManager _gameManager;

    public async Task CreateClient()
    {
        _gameManager = new ClientGameManager();
        await _gameManager.InitAsync();
    }
}
