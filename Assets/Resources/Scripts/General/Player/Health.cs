using Fusion;
using System;
using UnityEngine;

namespace General.Player{
    
    public class Health : NetworkBehaviour {
        [SerializeField] private int _maxHealth = 5;
        
        [Networked] public int HealthPoints { get; private set; }
        [Networked] public NetworkBool IsDead { get; private set; }
        
        public int MaxHealth => _maxHealth;
        public event Action<int, int> HealthChanged; // current, max — локальний UI
        public event Action Died;
        
        private ChangeDetector _changes;

        
        public override void Spawned()
        {
            _changes = GetChangeDetector(ChangeDetector.Source.SimulationState);
            if (HasStateAuthority)
            {
                HealthPoints = _maxHealth;
                IsDead = false;
            }
        }
        
        public override void Render()
        {
            foreach (var change in _changes.DetectChanges(this))
            {
                if (change == nameof(HealthPoints))
                    HealthChanged?.Invoke(HealthPoints, _maxHealth);
                if (change == nameof(IsDead) && IsDead)
                    Died?.Invoke();
            }
        }

        /// <summary>
        /// Викликати з Host (наприклад з BulletCollisionHandler), коли куля вже на State Authority.
        /// </summary>
        public void ApplyDamage(int amount, NetworkId attackerId)
        {
            if (!HasStateAuthority)
                return;
            if (IsDead || amount <= 0)
                return;
            // опційно: ігнорувати власні кулі (attackerId == Object.Id)
            if (attackerId == Object.Id)
                return;
            int newHealth = Mathf.Max(0, HealthPoints - amount);
            HealthPoints = newHealth;
            RPC_PlayHitFeedback(amount);
            if (newHealth <= 0)
                HandleDeath();
        }

        private void HandleDeath()
        {
            if (IsDead)
                return;
            IsDead = true;
            // тут: вимкнути рух, respawn через Runner, тощо — теж лише на HasStateAuthority
        }

        public void ResetHealth()
        {
            if (!HasStateAuthority)
                return;
            HealthPoints = _maxHealth;
            IsDead = false;
        }

        [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
        private void RPC_PlayHitFeedback(int damage)
        {
            // спрайт спалах, звук, camera shake — без зміни HP
            // Debug.Log($"Hit for {damage}, HP now replicated: {Health}");
        }
    }
}