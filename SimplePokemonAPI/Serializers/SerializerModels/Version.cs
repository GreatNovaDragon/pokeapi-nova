namespace SimplePokemonAPI.Serializers.SerializerModels;

public class Version
{
    public string ID { get; set; }
    public string Name { get; set; }
    public string InVersionGroupID { get; set; }
}

public class VersionGroup
{
    public string ID { get; set; }
    public int Order { get; set; }
}