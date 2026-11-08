using UnityEngine;
using Fusion;
using TMPro;

namespace UI.MiniGames
{
    public class Counter : NetworkBehaviour
    {
        [SerializeField] private TextMeshProUGUI _output;
        [Networked] public int CounterValue { get; private set; }
        
        

        public void Add(int value)
        {
            if (value <= 0) return;

            CounterValue += value;
        }


        public void Subtract(int value)
        {
            if(value <= 0) return;

            CounterValue -= value;
        }


        public override void Render()
        {
            base.Render();
            _output.text = CounterValue.ToString();
        }
    }
}