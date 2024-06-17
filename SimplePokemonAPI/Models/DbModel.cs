using PokeApiNet;
using Type = PokeApiNet.Type;

namespace SimplePokemonAPI.Models;

public class Database
{
    public Database(List<Pokemon> Pokemon, List<Attack> Attacks, List<Ability> Abilities, List<ElementalType> Types,
        List<DamageClass> DamageClasses, List<Effect> Effects)
    {
        this.Pokemon = Pokemon;
        this.Attacks = Attacks;
        this.Abilities = Abilities;
        this.Types = Types;
        this.DamageClasses = DamageClasses;
        this.Effects = Effects;
    }

    public Database()
    {
        Pokemon = [];
        Attacks = [];
        Abilities = [];
        Types = [];
        DamageClasses = [];
        Effects = [];
    }


    public List<Pokemon> Pokemon { get; set; }
    public List<Attack> Attacks { get; set; }
    public List<Ability> Abilities { get; set; }
    public List<ElementalType> Types { get; set; }
    public List<DamageClass> DamageClasses { get; set; }
    public List<Effect> Effects { get; set; }

    public async Task<Database> GetDatabase()
    {
        var apiclient = new PokeApiClient();
        await foreach (var TypeRessource in apiclient.GetAllNamedResourcesAsync<Type>())
        {
            var Types = new List<ElementalType>();

            var ApiType = await apiclient.GetResourceAsync(TypeRessource);
            Types.Add(
                new ElementalType
                {
                    ID = ApiType.Name,
                    Name = ApiType.Names.FirstOrDefault(n => n.Language.Name == "de").Name
                }
            );
        }

        foreach (var Type in Types)
        {
            var ApiType = await apiclient.GetResourceAsync<Type>(Type.ID);
            foreach (var dt in ApiType.DamageRelations.DoubleDamageTo)
                Type.DamageRelations.Add((Types.FirstOrDefault(t => t.ID == dt.Name), 200));

            foreach (var ht in ApiType.DamageRelations.HalfDamageTo)
                Type.DamageRelations.Add((Types.FirstOrDefault(t => t.ID == ht.Name), 50));

            foreach (var nt in ApiType.DamageRelations.NoDamageTo)
                Type.DamageRelations.Add((Types.FirstOrDefault(t => t.ID == nt.Name), 0));

            foreach (var RelationType in Types)
                if (!Type.DamageRelations.Any(e => e.DefendingType == RelationType))
                    Type.DamageRelations.Add((RelationType, 100));
        }

        return new Database();
    }
}