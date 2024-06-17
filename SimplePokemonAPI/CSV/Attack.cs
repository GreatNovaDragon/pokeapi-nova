namespace SimplePokemonAPI.CSV;

public class Attack
{
    public string ID { get; set; }
    public string Name { get; set; }
    public int Power { get; set; }
    public int PP { get; set; }
    public string DamageClassID { get; set; }
    public string EffectID { get; set; }
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
}

public class DamageRelations
{
    public string AttackerID { get; set; }
    public string DefenderID { get; set; }
    public int ProzentualMultiplier { get; set; }
}

public enum EffectType{ATTACK, ABILTY}

public class Effect
{
    public string ID { get; set; }
    public string Description { get; set; }
    public string Type { get; set; }
}