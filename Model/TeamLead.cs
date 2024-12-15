// Model/TeamLead.cs
namespace Model;

// Класс для тимлидов
public class TeamLead : Person
{
    public TeamLead(int id, string name, List<int> preferences) : base(id, name, preferences) { }
}
