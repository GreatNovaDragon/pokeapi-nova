using SimplePokemonAPI.Models;

namespace SimplePokemonAPI.FileModels;

public abstract class FileModel
{
    public List<Pokemon> Pokemon { get; set; } = [];
    public List<PokemonAbility> PokemonAbility { get; set; } = [];
    public List<Ability> Abilities { get; set; } = [];
    public List<Effect> Effects { get; set; } = [];
    public List<ElementalType> Types { get; set; } = [];
    public List<DamageRelations> DamageRelations { get; set; } = [];
    public List<DamageClass> DamageClasses { get; set; } = [];
    public List<Attack> Attacks { get; set; } = [];
    public List<PokemonAttack> Learnsets { get; set; } = [];

    public abstract void Read();

    public abstract void Write();

    public abstract void PrepareEmptyDatabase();


    public static void OverwriteWithDataFromDB(Database db)
    {
        throw new NotImplementedException();
    }
}