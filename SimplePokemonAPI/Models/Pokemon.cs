namespace SimplePokemonAPI.Models;

public class Pokemon
{
    public string ID { get; set; }
    public string Name { get; set; }
    public StatBlock Stats { get; set; }
    public List<(Ability Ability, Boolean IsHidden)> Abilities { get; set; }
    public List<PokemonAttack> Learnset { get; set; }
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
    public Effect? Effect { get; set; }
}

public class PokemonAttack
{
    public Attack Attack { get; set; }
    public string Trigger { get; set; }
    public string TriggerDetails { get; set; }
}