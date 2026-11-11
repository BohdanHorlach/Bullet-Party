using Fusion;
using General.MiniGames;
using General.Player;
using General.Utils;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace UI.Lobby
{
    [System.Serializable]
    public struct TextByPlayerType
    {
        public PlayerType Type;
        public TextMeshProUGUI Text;
    }

    public class Ranking : MonoBehaviour
    {
        [SerializeField] private TextByPlayerType[] _rankingTexts;

        private Dictionary<PlayerType, TextMeshProUGUI> _textByTypes;


        private void Awake()
        {
            _textByTypes = DictionaryInitializer.InstantiateDictionary
                (_rankingTexts,
                (element) => element.Type,
                (element) => element.Text);
        }


        private void OnEnable()
        {
            NetworkDictionary<PlayerType, int> ranking = MatchRanking.Instance.Ranking;

            foreach(KeyValuePair<PlayerType, int> rank in ranking)
            {
                PlayerType playerType = rank.Key;
                TextMeshProUGUI text = _textByTypes[playerType];

                text.text = rank.Value.ToString();
            }
        }
    }
}
