using UnityEngine;

public class GameStateMenu : GameStateBase
{
    public override void OnStateEnter(params object[] objs)
    {
        base.OnStateEnter(objs);
        manager.InputReader.Controls.UI.Enable();
    }

    public override void OnStateExit()
    {
        manager.InputReader.Controls.UI.Disable();
    }
}