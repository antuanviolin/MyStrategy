using Nsu.HackathonProblem;
using Nsu.HackathonProblem.Contracts;

namespace MyStrategy;

class Program
{
    static void Main(string[] args)
    {
        var random = new Random();
        const int hackathonRuns = 1;

        var juniors = LoadEmployeesFromCsv("Juniors20.csv");
        var teamLeads = LoadEmployeesFromCsv("Teamleads20.csv");

        var strategy = new MyInterfaceStrategy();
        var harmonicMeans = new List<double>();

        for (int run = 0; run < hackathonRuns; run++)
        {
            var teamLeadsWishlists = teamLeads.Select(tl =>
                new Wishlist(tl.Id, GenerateRandomWishlist(juniors.Select(j => j.Id).ToArray(), random)));

            var juniorsWishlists = juniors.Select(j =>
                new Wishlist(j.Id, GenerateRandomWishlist(teamLeads.Select(tl => tl.Id).ToArray(), random)));

            var teams = strategy.BuildTeams(teamLeads, juniors, teamLeadsWishlists, juniorsWishlists);

            double harmonicMean = ComputeHarmonicity(teams, teamLeadsWishlists, juniorsWishlists);
            Console.WriteLine($"Harmonic Mean for Run {run + 1}: {harmonicMean:F2}");
            harmonicMeans.Add(harmonicMean);
        }

        Console.WriteLine($"Average Harmonic Satisfaction: {harmonicMeans.Average():F2}");
    }

    private static int[] GenerateRandomWishlist(int[] options, Random random)
    {
        return options.OrderBy(_ => random.Next()).ToArray();
    }


    private static double ComputeHarmonicity(
        IEnumerable<Team> teams,
        IEnumerable<Wishlist> teamLeadsWishlists,
        IEnumerable<Wishlist> juniorsWishlists)
    {
        var participants = new List<(string Name, List<int> WishList, int AssignedPartner, int SatisfactionIndex)>();

        foreach (var team in teams)
        {
            var teamLeadWishlist = teamLeadsWishlists.First(w => w.EmployeeId == team.TeamLead.Id).DesiredEmployees
                .ToList();
            var juniorWishlist = juniorsWishlists.First(w => w.EmployeeId == team.Junior.Id).DesiredEmployees.ToList();

            participants.Add((
                team.TeamLead.Name,
                teamLeadWishlist,
                team.Junior.Id,
                CalculateSatisfactionIndex(teamLeadWishlist, team.Junior.Id)));

            participants.Add((
                team.Junior.Name,
                juniorWishlist,
                team.TeamLead.Id,
                CalculateSatisfactionIndex(juniorWishlist, team.TeamLead.Id)));
        }

        return ComputeHarmonicity(participants);
    }

    private static int CalculateSatisfactionIndex(List<int> wishList, int assignedPartner)
    {
        int position = wishList.IndexOf(assignedPartner);
        return 20 - position;
    }

    private static double ComputeHarmonicity(
        List<(string Name, List<int> WishList, int AssignedPartner, int SatisfactionIndex)> participants)
    {
        int n = participants.Count;
        double denominator = 0;

        foreach (var participant in participants)
        {
            denominator += 1.0 / participant.SatisfactionIndex;
        }

        return n / denominator;
    }

    private static IEnumerable<Employee> LoadEmployeesFromCsv(string filePath)
    {
        return File.ReadLines(filePath)
            .Skip(1)
            .Select(line => line.Split(';'))
            .Select(parts => new Employee(int.Parse(parts[0]), parts[1]))
            .ToList();
    }
}