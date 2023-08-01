namespace ZombieProject.Core
{
    public class EnemyStateWandering : EnemyStateBase
    {
        public override void OnStateEnter(params object[] objs)
        {
            base.OnStateEnter(objs);
            enemy.MovementComponent.StartWandering();
        }

        public override void OnStateStay()
        {
            enemy.MovementComponent.HandleWandering();
        }

        public override void OnStateExit()
        {
            enemy.MovementComponent.StopWandering();
        }
    }
}