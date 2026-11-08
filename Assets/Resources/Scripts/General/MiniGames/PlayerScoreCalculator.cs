
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Fusion;

namespace General.MiniGames
{
    public static class PlayerScoreCalculator
    {
        private static readonly int[] NormalizedScores =
        {
        500, 250, 100, 0
    };

        public static Dictionary<PlayerRef, int> Normalize(
            Dictionary<PlayerRef, int> scores)
        {
            if (scores == null || scores.Count != 4)
                return new Dictionary<PlayerRef, int>();

            var sortedPlayers = scores
                .OrderByDescending(x => x.Value)
                .ToList();

            var result = new Dictionary<PlayerRef, int>();

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