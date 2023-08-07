using UnityEngine;

public class GameStateGameplay : GameStateBase
{
    public override void OnStateEnter(params object[] objs)
    {
        base.OnStateEnter(objs);
        manager.InputReader.Controls.Gameplay.Enable();
    }

    public override void OnStateExit()
    {
        manager.InputReader.Controls.Gameplay.Disable();
    }
}