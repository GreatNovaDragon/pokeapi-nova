namespace SimplePokemonAPI.Models;

public class Move
{
    public string ID { get; set; }
    public string Name { get; set; }
    public int? Power { get; set; }
    public int? PP { get; set; }

    public int? Accuracy { get; set; }
    public int? Priority { get; set; }
    public DamageClass? DamageClass { get; set; }

    public int? EffectChance { get; set; }
    public Effect? Effect { get; set; }
}

public class DamageClass
{
    public string ID { get; set; }
    public string Name { get; set; }
}

public class ElementalType
{
    public string ID { get; set; }
    public string Name { get; set; }
    public List<(ElementalType DefendingType, int ProcentualMultiplier)> DamageRelations { get; set; }
}

public class Effect
{
    public string ID { get; set; }
    public string Description { get; set; }
}