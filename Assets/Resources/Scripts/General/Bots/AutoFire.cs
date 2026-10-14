using General.Combat;


namespace General.Bots
{
    public class AutoFire : FireHandler
    {
        protected override void OnEnable()
        {
            StartFiring(transform.up);
        }


        protected override void OnDisable() 
        {
            StopFiring();
        }


        private void Update()
        {
            _currentDirection = transform.up;
        }
    }
}