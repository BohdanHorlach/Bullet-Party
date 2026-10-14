using Fusion;

namespace General.Match.PlayerPlacement
{
    public class BaseSpawnPointAssigner : NetworkBehaviour
    {
        public virtual void SetPlace(PlayerRef playerRef) { }
    }
}