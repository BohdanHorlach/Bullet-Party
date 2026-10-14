using Fusion;
using General.Match.StateManagement;


namespace General.Player
{
    public class PlayerNetwork : NetworkBehaviour
    {
        public static PlayerNetwork Local { get; private set; }


        public override void Spawned()
        {
            if (Object.HasInputAuthority)
                Local = this;
        }


        public override void Despawned(NetworkRunner runner, bool hasState)
        {
            if (Local == this)
                Local = null;
        }


        [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
        public void RPC_SetReady()
        {
            ReadyManager.Instance.SetReady(Object.InputAuthority);
        }
    }
}