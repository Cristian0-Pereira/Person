namespace Person.Models;

public class PersonModel(string name)
{
    public Guid Id { get; init; } = Guid.NewGuid();
    public string Name { get; private set; } = name;
}
