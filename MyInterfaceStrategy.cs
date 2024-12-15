using Nsu.HackathonProblem.Contracts;
using Model;

namespace MyStrategy;

public class MyInterfaceStrategy : ITeamBuildingStrategy
{
    public IEnumerable<Team> BuildTeams(IEnumerable<Employee> teamLeads, IEnumerable<Employee> juniors,
        IEnumerable<Wishlist> teamLeadsWishlists, IEnumerable<Wishlist> juniorsWishlists)
    {
        var myteamLeads = new List<TeamLead>();
        var myjuniors = new List<Junior>();
        var wish = new List<int>();
        foreach (var teamLead in teamLeads)
        {
            foreach (var wishteamlead in teamLeadsWishlists)
            {
                if (wishteamlead.EmployeeId == teamLead.Id)
                {
                    wish = wishteamlead.DesiredEmployees.ToList();
                }
            }
            myteamLeads.Add(new TeamLead(teamLead.Id, teamLead.Name, wish));
        }

        foreach (var junior in juniors)
        {
            foreach (var wishjunior in juniorsWishlists)
            {
                if (wishjunior.EmployeeId == junior.Id)
                {
                    wish = wishjunior.DesiredEmployees.ToList();
                }
            }
            myjuniors.Add(new Junior(junior.Id, junior.Name, wish));
        }

        var myengagements = GaleShapley.ConductStableMatching(myjuniors, myteamLeads);

        var satisfactionIndices = CalculateSatisfactionIndices(myengagements);

        var mean = CalculateHarmonicMean(satisfactionIndices);

        Console.WriteLine($"Harmonic Mean for Run: {mean:F2}");

        var myteams = new List<Team>();
        foreach (var engagement in myengagements)
        {
            myteams.Add(new Team(new Employee(engagement.Key.Id, engagement.Key.Name), new Employee(engagement.Value.Id, engagement.Value.Name)));
        }
        return myteams;
    }

    public double CalculateHarmonicMean(List<int> satisfactionIndices)
    {
        int n = satisfactionIndices.Count;
        double sumOfReciprocals = 0;

        foreach (var index in satisfactionIndices)
        {
            if (index > 0)
            {
                sumOfReciprocals += 1.0 / index;
            }
        }

        return n / sumOfReciprocals;
    }

    public List<int> CalculateSatisfactionIndices(Dictionary<Junior, TeamLead> stablePairs)
    {
        var satisfactionIndices = new List<int>();
        foreach (var pair in stablePairs)
        {
            var junior = pair.Key;
            var teamLead = pair.Value;

            int juniorSatisfaction = junior.GetSatisfactionScore(teamLead.Id);
            int teamLeadSatisfaction = teamLead.GetSatisfactionScore(junior.Id);

            satisfactionIndices.Add(juniorSatisfaction);
            satisfactionIndices.Add(teamLeadSatisfaction);
        }

        return satisfactionIndices;
    }
}
