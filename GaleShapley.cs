using Model;

namespace MyStrategy;

public class GaleShapley
{

    public static Dictionary<Junior, TeamLead> ConductStableMatching(List<Junior> juniors, List<TeamLead> teamLeads)
    {
        int n = juniors.Count; // Количество джунов и тимлидов (предполагается одинаковое)
        int[,] costMatrix = BuildCostMatrix(juniors, teamLeads);

        // Применяем венгерский алгоритм для поиска оптимального назначения
        int[] assignment = HungarianAlgorithm(costMatrix);

        // Формируем результат
        var engagements = new Dictionary<Junior, TeamLead>();
        for (int i = 0; i < n; i++)
        {
            engagements[juniors[i]] = teamLeads[assignment[i]];
        }

        return engagements;
    }

    private static int[,] BuildCostMatrix(List<Junior> juniors, List<TeamLead> teamLeads)
    {
        int n = juniors.Count;
        int[,] costMatrix = new int[n, n];

        for (int i = 0; i < n; i++)
        {
            for (int j = 0; j < n; j++)
            {
                // Считаем "стоимость" как сумму удовлетворенностей джуна и тимлида
                int juniorScore = juniors[i].GetSatisfactionScore(teamLeads[j].Id);
                int teamLeadScore = teamLeads[j].GetSatisfactionScore(juniors[i].Id);
                costMatrix[i, j] = -(juniorScore + teamLeadScore); // Для максимизации берем отрицательное значение
            }
        }
        return costMatrix;
    }

    private static int[] HungarianAlgorithm(int[,] costMatrix)
    {
        int n = costMatrix.GetLength(0);
        int[] u = new int[n]; // Потенциалы по строкам
        int[] v = new int[n]; // Потенциалы по столбцам
        int[] p = new int[n]; // Покрытие по столбцам
        int[] way = new int[n]; // "Путь" для минимального покрытия

        for (int i = 0; i < n; i++)
        {
            int[] minv = new int[n];
            bool[] used = new bool[n];
            Array.Fill(minv, int.MaxValue);
            int j0 = 0; // Текущий столбец
            p[0] = i;

            do
            {
                used[j0] = true;
                int i0 = p[j0], delta = int.MaxValue, j1 = 0;

                for (int j = 1; j < n; j++)
                {
                    if (!used[j])
                    {
                        int cur = costMatrix[i0, j] - u[i0] - v[j];
                        if (cur < minv[j])
                        {
                            minv[j] = cur;
                            way[j] = j0;
                        }
                        if (minv[j] < delta)
                        {
                            delta = minv[j];
                            j1 = j;
                        }
                    }
                }

                for (int j = 0; j < n; j++)
                {
                    if (used[j])
                    {
                        u[p[j]] += delta;
                        v[j] -= delta;
                    }
                    else
                    {
                        minv[j] -= delta;
                    }
                }
                j0 = j1;
            } while (p[j0] != 0);

            do
            {
                int j1 = way[j0];
                p[j0] = p[j1];
                j0 = j1;
            } while (j0 != 0);
        }

        int[] result = new int[n];
        for (int j = 1; j < n; j++)
        {
            result[p[j]] = j;
        }
        return result;
    }
}
