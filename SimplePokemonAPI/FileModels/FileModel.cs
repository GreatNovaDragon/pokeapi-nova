using SimplePokemonAPI.Models;

namespace SimplePokemonAPI.FileModels;

public class FileModel
{
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

    public virtual void Read()
    {
        throw new NotImplementedException();
    }

    public virtual void Write()
    {
        throw new NotImplementedException();
    }

    public virtual void PrepareEmptyDatabase()
    {
        throw new NotImplementedException();
    }

    public void InsertFromDBModel(Database db)
    {
        throw new NotImplementedException();

    }

    public List<Pokemon> Pokemon { get; set; }
    public List<PokemonAbility> PokemonAbility { get; set; }
    public List<Ability> Abilities { get; set; }
    public List<Effect> Effects { get; set; }
    public List<ElementalType> Types { get; set; }
    public List<DamageRelations> DamageRelations { get; set; }
    public List<DamageClass> DamageClasses { get; set; }
    public List<Attack> Attacks { get; set; }
    public List<PokemonAttack> Learnsets { get; set; }
}