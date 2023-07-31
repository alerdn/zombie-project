using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class HostSingleton : PersistentSingleton<HostSingleton>
{
    private HostGameManager _gameManager;

    public void CreateHost()
    {
        _gameManager = new HostGameManager();
    }
}
