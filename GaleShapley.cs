using Model;

namespace MyStrategy;

public class GaleShapley
{
    public static Dictionary<Junior, TeamLead> ConductStableMatching(List<Junior> juniors, List<TeamLead> teamLeads)
    {
         // Свободные тимлиды
        var freeTeamLeads = new Queue<TeamLead>(teamLeads);

        // Карты для отслеживания предложений
        var proposals = new Dictionary<TeamLead, HashSet<int>>();
        foreach (var teamLead in teamLeads)
        {
            proposals[teamLead] = new HashSet<int>();
        }

        // Сопоставление джунов с тимлидами
        var engagements = new Dictionary<Junior, TeamLead>();

        while (freeTeamLeads.Count > 0)
        {
            var teamLead = freeTeamLeads.Dequeue();
            var teamLeadPreferences = teamLead.Preferences;

            foreach (var juniorId in teamLeadPreferences)
            {
                // Если тимлид еще не предлагал этому джуну
                if (!proposals[teamLead].Contains(juniorId))
                {
                    proposals[teamLead].Add(juniorId);
                    var junior = juniors.First(j => j.Id == juniorId);

                    if (!engagements.ContainsKey(junior))
                    {
                        // Если джун еще не обручен
                        engagements[junior] = teamLead;
                    }
                    else
                    {
                        var currentTeamLead = engagements[junior];
                        // Проверяем, предпочитает ли джун нового тимлида текущему
                        if (junior.Preferences.IndexOf(teamLead.Id) <
                            junior.Preferences.IndexOf(currentTeamLead.Id))
                        {
                            // Джун предпочитает нового тимлида
                            engagements[junior] = teamLead;
                            freeTeamLeads.Enqueue(currentTeamLead); // Текущий тимлид становится свободным
                        }
                        else
                        {
                            // Джун остается с текущим тимлидом, а новый тимлид свободен
                            freeTeamLeads.Enqueue(teamLead);
                        }
                    }
                    break; // Тимлид делает предложение одному джуну за раз
                }
            }
        }
        return engagements;
    }
}
