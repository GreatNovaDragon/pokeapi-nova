namespace SimplePokemonAPI.Models;

public class Attack
{
    public string ID { get; set; }
    public string Name { get; set; }
    public int? Power { get; set; }
    public int? PP { get; set; }
    public DamageClass DamageClass { get; set; }
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

public enum EffectType
{
    ATTACK,
    ABILTY
}

public class Effect
{
    public string ID { get; set; }
    public string Description { get; set; }
    public EffectType Type { get; set; }
}