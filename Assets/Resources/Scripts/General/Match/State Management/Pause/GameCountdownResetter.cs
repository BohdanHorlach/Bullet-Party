using Fusion;


namespace General.Match.StateManagement
{
    public class GameCountdownResetter : NetworkBehaviour
    {
        public override void Spawned()
        {
            base.Spawned();

            if (HasStateAuthority)
                GamePause.SetPaused(false);
        }
    }
}
