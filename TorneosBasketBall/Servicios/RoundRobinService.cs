using System;
using System.Collections.Generic;
using TorneosBasketBall.Models;

namespace TorneosBasketBall.Services
{
    public class RoundRobinService
    {
        private Random _random = new Random();

        /// <summary>
        /// Generates a single-round robin schedule (each team plays each other once).
        /// </summary>
        public List<Partidos> GenerateRoundRobin(List<int> equipoIds, DateTime startDate)
        {
            var partidos = new List<Partidos>();
            int n = equipoIds.Count;
            bool isOdd = n % 2 != 0;
            if (isOdd)
            {
                equipoIds.Add(-1); // bye placeholder
                n++;
            }

            int rounds = n - 1;
            int matchesPerRound = n / 2;
            var teams = new List<int>(equipoIds);

            for (int round = 0; round < rounds; round++)
            {
                DateTime matchDate = startDate.AddDays(round);
                for (int i = 0; i < matchesPerRound; i++)
                {
                    int home = teams[i];
                    int away = teams[n - 1 - i];
                    if (home == -1 || away == -1) continue;

                    // Generate random scores
                    int homeScore = _random.Next(70, 120); // Example: scores between 70 and 119
                    int awayScore = _random.Next(70, 120);

                    // Ensure there's a winner or a tie for simplicity for now
                    if (homeScore == awayScore)
                    {
                        if (_random.Next(0, 2) == 0) homeScore++; // Randomly make one team win if tied
                        else awayScore++;
                    }

                    partidos.Add(new Partidos
                    {
                        EquipoLocalID = home,
                        EquipoVisitanteID = away,
                        FechaHora = matchDate,
                        Estado = "Finalizado", // Mark as finished since scores are generated
                        PuntuacionLocal = homeScore,
                        PuntuacionVisitante = awayScore
                    });
                }
                // rotate
                int last = teams[teams.Count - 1];
                teams.RemoveAt(teams.Count - 1);
                teams.Insert(1, last);
            }

            return partidos;
        }
    }
}