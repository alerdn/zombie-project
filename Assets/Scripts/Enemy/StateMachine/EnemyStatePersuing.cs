public class EnemyStatePersuing : EnemyStateBase
{
    public override void OnStateEnter(params object[] objs)
    {
        base.OnStateEnter(objs);
        enemy.MovementComponent.StartPersuing();
    }

    public override void OnStateStay()
    {
        enemy.MovementComponent.HandlePersuing();
    }

    public override void OnStateExit()
    {
        enemy.MovementComponent.StopPersuing();
    }
}
