using SimplePokemonAPI.Models;

namespace SimplePokemonAPI.FileModels;

public abstract class DatabaseModel
{
    public List<Pokemon> Pokemon { get; set; } = [];
    public List<PokemonAbility> PokemonAbility { get; set; } = [];
    public List<Ability> Abilities { get; set; } = [];
    public List<Effect> MoveEffects { get; set; } = [];
    public List<Effect> AbilityEffects { get; set; } = [];

    public List<ElementalType> Types { get; set; } = [];
    public List<DamageRelations> DamageRelations { get; set; } = [];
    public List<DamageClass> DamageClasses { get; set; } = [];
    public List<Move> Moves { get; set; } = [];
    public List<PokemonAttack> Learnsets { get; set; } = [];

    public abstract void Read();

    public abstract void Write();

    public abstract void PrepareEmpty();


    public void OverwriteWithDataFromDB(Knowledgebase db)
    {
        throw new NotImplementedException();
    }
}