namespace General.MiniGames.JumpIt
{
    public interface IJumpItTrap
    {
        bool IsBusy { get; }
        void ActivateTrap();
        bool TryActivateTrap();
    }
}