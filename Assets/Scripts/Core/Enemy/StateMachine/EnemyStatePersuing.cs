namespace ZombieProject.Core
{
    public class EnemyStatePersuing : EnemyStateBase
    {
        private Player _player;

        public override void OnStateEnter(params object[] objs)
        {
            base.OnStateEnter(objs);
            _player = (Player)((object[])objs[1])[0];
        }

        public override void OnStateStay()
        {
            enemy.MovementComponent.HandlePersuing(_player);
        }

        public override void OnStateExit()
        {
            enemy.MovementComponent.StopPersuing();
        }
    }
}