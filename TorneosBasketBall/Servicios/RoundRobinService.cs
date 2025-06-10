using TorneosBasketBall.Models;

namespace TorneosBasketBall.Services
{
    public class RoundRobinService
    {
        private Random _random = new Random();
        public List<Partidos> GenerateRoundRobin(List<int> equipoIds, DateTime startDate)
        {
            var partidos = new List<Partidos>();
            int n = equipoIds.Count;
            bool isOdd = n % 2 != 0;
            if (isOdd)
            {
                equipoIds.Add(-1);
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

                    int homeScore = _random.Next(70, 120);
                    int awayScore = _random.Next(70, 120);

                    if (homeScore == awayScore)
                    {
                        if (_random.Next(0, 2) == 0) homeScore++;
                        else awayScore++;
                    }

                    partidos.Add(new Partidos
                    {
                        EquipoLocalID = home,
                        EquipoVisitanteID = away,
                        FechaHora = matchDate,
                        Estado = "Finalizado",
                        PuntuacionLocal = homeScore,
                        PuntuacionVisitante = awayScore
                    });
                }
                
                int last = teams[teams.Count - 1];
                teams.RemoveAt(teams.Count - 1);
                teams.Insert(1, last);
            }

            return partidos;
        }
    }
}