namespace SimplePokemonAPI.Models;

public class Version
{
    public string ID { get; set; }
    public string Name { get; set; }
    public VersionGroup InVersionGroup { get; set; }
}

public class VersionGroup
{
    public string ID { get; set; }
    public int Order { get; set; }
}