using SimplePokemonAPI.Models;

namespace SimplePokemonAPI.FileModels;

public class FileModel
{
    public List<Pokemon> Pokemon { get; set; }
    public List<PokemonAbility> PokemonAbility { get; set; } 
    public List<Ability> Abilities { get; set; }
    public List<Effect> Effects { get; set; } 
    public List<ElementalType> Types { get; set; }
    public List<DamageRelations> DamageRelations { get; set; }
    public List<DamageClass> DamageClasses { get; set; } 
    public List<Attack> Attacks { get; set; }
    public List<PokemonAttack> Learnsets { get; set; }

    protected FileModel()
    {
        Pokemon = [];
        PokemonAbility = [];
        Abilities = [];
        Effects = [];
        Types = [];
        DamageRelations = [];
        DamageClasses = [];
        Attacks = [];
        Learnsets = [];
    }
    
    public FileModel(Database db) : this()
    {
        foreach (var pkmn in db.Pokemon)
        {
            Pokemon.Add(new Pokemon
            {
                ID = pkmn.ID,
                Name = pkmn.Name,
                FormName = pkmn.FormName,
                Stats = new StatBlock
                {
                    Attack = pkmn.Stats.Attack, SpecialAttack = pkmn.Stats.SpecialAttack, Defense = pkmn.Stats.Defense,
                    SpecialDefense = pkmn.Stats.SpecialDefense, Speed = pkmn.Stats.Speed, HP = pkmn.Stats.HP
                }
            });
        }

        foreach (var a in db.Abilities)
        {
            Abilities.Add(new Ability
            {
                ID = a.ID,
                Name = a.Name,
                EffectID = a.Effect != null ? a.Effect.ID : "",
            });
        }

        foreach (var t in db.Types)
        {
            Types.Add(new ElementalType
            {
                ID = t.ID,
                Name = t.Name,
            });
            foreach (var dr in t.DamageRelations)
            {
                DamageRelations.Add(new DamageRelations
                {
                    AttackerID = t.ID, DefenderID = dr.DefendingType.ID, ProzentualMultiplier = dr.ProcentualMultiplier
                });
            }
        }
    }

}