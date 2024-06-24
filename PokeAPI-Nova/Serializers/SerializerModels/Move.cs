namespace SimplePokemonAPI.Serializers.SerializerModels;

public class Move
{
    public string? ID { get; set; }
    public string? Name { get; set; }
    public int? Power { get; set; }
    public int? PP { get; set; }

    public int? Accuracy { get; set; }
    public int? Priority { get; set; }

    public string? DamageClassID { get; set; }

    public int? EffectChance { get; set; }

    public string? EffectID { get; set; }

    public string? TypeID { get; set; }
    public string? IntroducedInVersionGroupID { get; set; }
}

public class DamageClass
{
    public string ID { get; set; }
    public string Name { get; set; }
}

public class ElementalType
{
    public string? ID { get; set; }
    public string Name { get; set; }

    public string IntroducedInVersionGroupID { get; set; }
}

public class DamageRelation
{
    public string? AttackerID { get; set; }
    public string? DefenderID { get; set; }
    public int ProzentualMultiplier { get; set; }
}

public class Effect
{
    public string ID { get; set; }
    public string Description { get; set; }
}