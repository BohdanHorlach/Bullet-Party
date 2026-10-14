using Fusion;


namespace General.Player
{
    public class PlayerAvatar : NetworkBehaviour
    {
        [Networked] public PlayerType PlayerType { get; set; }
        [Networked] public int Health { get; set; }
    }
}