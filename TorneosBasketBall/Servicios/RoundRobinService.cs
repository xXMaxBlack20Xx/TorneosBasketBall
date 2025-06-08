using TorneosBasketBall.Models; // Adjust namespace to match your project
using System;
using System.Collections.Generic;

namespace TorneosBasketBall.Services
{
    public class RoundRobinService
    {
        public List<Partidos> GenerateRoundRobin(List<int> equipoIds, DateTime startDate)
        {
            var partidos = new List<Partidos>();
            int numEquipos = equipoIds.Count;
            bool isOdd = numEquipos % 2 != 0;

            if (isOdd)
            {
                equipoIds.Add(-1); // Dummy team ID
                numEquipos++;
            }

            int numJornadas = numEquipos - 1;
            int partidosPorJornada = numEquipos / 2;

            var equipos = new List<int>(equipoIds);
            DateTime fecha = startDate;

            for (int ronda = 0; ronda < numJornadas; ronda++)
            {
                for (int i = 0; i < partidosPorJornada; i++)
                {
                    int local = equipos[i];
                    int visitante = equipos[numEquipos - 1 - i];

                    if (local == -1 || visitante == -1)
                        continue;

                    partidos.Add(new Partidos
                    {
                        EquipoLocalID = local,
                        EquipoVisitanteID = visitante,
                        FechaHora = fecha.AddDays(ronda),
                        Estado = "Programado",
                        PuntuacionLocal = null,
                        PuntuacionVisitante = null
                    });
                }

                var temp = equipos[1];
                equipos.RemoveAt(1);
                equipos.Add(temp);
            }

            return partidos;
        }
    }
}
