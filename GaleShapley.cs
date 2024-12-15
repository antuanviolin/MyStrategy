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

    public static Dictionary<TeamLead, Junior> ConductStableMatching(List<TeamLead> teamLeads, List<Junior> juniors)
    {
        // Свободные джуны
        var freeJuniors = new Queue<Junior>(juniors);

        // Карты для отслеживания предложений
        var proposals = new Dictionary<Junior, HashSet<int>>();
        foreach (var junior in juniors)
        {
            proposals[junior] = new HashSet<int>();
        }

        // Сопоставление тимлидов с джунами
        var engagements = new Dictionary<TeamLead, Junior>();

        while (freeJuniors.Count > 0)
        {
            var junior = freeJuniors.Dequeue();
            var juniorPreferences = junior.Preferences;

            foreach (var teamLeadId in juniorPreferences)
            {
                // Если джун еще не предлагал этому тимлиду
                if (!proposals[junior].Contains(teamLeadId))
                {
                    proposals[junior].Add(teamLeadId);
                    var teamLead = teamLeads.First(tl => tl.Id == teamLeadId);

                    if (!engagements.ContainsKey(teamLead))
                    {
                        // Если тимлид еще не обручен
                        engagements[teamLead] = junior;
                    }
                    else
                    {
                        var currentJunior = engagements[teamLead];
                        // Проверяем, предпочитает ли тимлид нового джуна текущему
                        if (teamLead.Preferences.IndexOf(junior.Id) < teamLead.Preferences.IndexOf(currentJunior.Id))
                        {
                            // Тимлид предпочитает нового джуна
                            engagements[teamLead] = junior;
                            freeJuniors.Enqueue(currentJunior); // Текущий джун становится свободным
                        }
                        else
                        {
                            // Тимлид остается с текущим джуном, а новый джун свободен
                            freeJuniors.Enqueue(junior);
                        }
                    }
                    break; // Джун делает предложение одному тимлиду за раз
                }
            }
        }

        return engagements;
    }
}
