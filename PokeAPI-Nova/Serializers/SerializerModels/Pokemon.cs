namespace SimplePokemonAPI.Serializers.SerializerModels;

public class Pokemon
{
    public string ID { get; set; }
    public string Name { get; set; }
    public string FormName { get; set; }
    public StatBlock Stats { get; set; }

    public string? PrimaryTypeID { get; set; }
    public string? SecundaryTypeID { get; set; }
    public string IntroducedInVersionGroupID { get; set; }
}

public class VisualOnlyPokemon
{
    public string ID { get; set; }
    public string Name { get; set; }
    public string FormName { get; set; }

    public string IntroducedInVersionGroupID { get; set; }

    public string basedOnPokemonID { get; set; }
}

public class PokemonAbility
{
    public string PokemonID { get; set; }
    public string AbilityID { get; set; }
    public int Slot { get; set; }
    public bool IsHidden { get; set; }
}

public class StatBlock
{
    public int HP { get; set; }
    public int Attack { get; set; }
    public int Defense { get; set; }
    public int SpecialAttack { get; set; }
    public int SpecialDefense { get; set; }
    public int Speed { get; set; }
}

public class Ability
{
    public string ID { get; set; }
    public string Name { get; set; }
    public string? EffectID { get; set; }
    public string? IntroducedInVersionGroupID { get; set; }
}

public class PokemonAttack
{
    public string PokemonID { get; set; }
    public string AttackID { get; set; }
    public string Trigger { get; set; }
    public string TriggerDetails { get; set; }
    
    public string AppliesToVersionGroupID { get; set; }

}