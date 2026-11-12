
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using General.Player;

namespace General.MiniGames
{
    public static class PlayerScoreCalculator
    {
        private static readonly int[] NormalizedScores =
        {
            500, 250, 100, 0
        };

        private static readonly PlayerType[] RankedPlayerTypes =
        {
            PlayerType.Red,
            PlayerType.Yellow,
            PlayerType.Blue,
            PlayerType.Green,
        };

        public static Dictionary<PlayerType, int> Normalize(
            Dictionary<PlayerType, int> scores)
        {
            var filled = new Dictionary<PlayerType, int>(RankedPlayerTypes.Length);

            foreach (PlayerType type in RankedPlayerTypes)
            {
                filled[type] = scores != null && scores.TryGetValue(type, out int value)
                    ? value
                    : 0;
            }

            var sortedPlayers = filled
                .OrderByDescending(x => x.Value)
                .ToList();

            var result = new Dictionary<PlayerType, int>();

            int index = 0;

            while (index < sortedPlayers.Count)
            {
                int endIndex = index + 1;

                while (endIndex < sortedPlayers.Count &&
                       sortedPlayers[endIndex].Value == sortedPlayers[index].Value)
                {
                    endIndex++;
                }

                int totalScore = 0;

                for (int i = index; i < endIndex; i++)
                {
                    totalScore += NormalizedScores[i];
                }

                int sharedScore = Mathf.FloorToInt(
                    (float)totalScore / (endIndex - index)
                );

                for (int i = index; i < endIndex; i++)
                {
                    result[sortedPlayers[i].Key] = sharedScore;
                }

                index = endIndex;
            }

            return result;
        }
    }

}