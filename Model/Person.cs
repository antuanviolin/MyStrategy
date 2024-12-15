// Model/Person.cs
namespace Model;

// Класс для участников
public class Person
{
    public int Id { get; set; }
    public string Name { get; set; }
    public List<int> Preferences { get; set; }

    public Person(int id, string name, List<int> preferences)
    {
        Id = id;
        Name = name;
        Preferences = preferences;
    }

    public int GetSatisfactionScore(int partnerId)
    {
        int rank = Preferences.IndexOf(partnerId);
        if (rank == -1)
            return 0;
        return Preferences.Count - rank;
    }
}