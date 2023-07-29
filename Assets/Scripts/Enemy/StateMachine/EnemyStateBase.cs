public class EnemyStateBase : StateBase
{
    protected EnemyBase enemy;

    public override void OnStateEnter(params object[] objs)
    {
        enemy = (EnemyBase)objs[0];
    }
}
