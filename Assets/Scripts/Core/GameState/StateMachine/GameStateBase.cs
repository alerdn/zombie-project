using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameStateBase : StateBase
{
    protected GameStateManager manager;

    public override void OnStateEnter(params object[] objs)
    {
        manager = (GameStateManager)objs[0];
    }

}
