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

        var Types = new List<ElementalType>();

        await foreach (var TypeRessource in apiclient.GetAllNamedResourcesAsync<Type>())
        {
            var ApiType = await apiclient.GetResourceAsync(TypeRessource);
            Types.Add(
                new ElementalType
                {
                    ID = ApiType.Name,
                    Name = ApiType.Names.FirstOrDefault(n => n.Language.Name == "en").Name
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

        var DamageClasses = new List<DamageClass>();

        await foreach (var dc in apiclient.GetAllNamedResourcesAsync<MoveDamageClass>())
        {
            var API_damageclass = await apiclient.GetResourceAsync(dc);
            DamageClasses.Add(
                new DamageClass
                {
                    ID = API_damageclass.Name,
                    Name = API_damageclass.Names.FirstOrDefault(n => n.Language.Name == "en").Name
                });
        }

        var Moves = new List<Attack>();

        await foreach (var m in apiclient.GetAllNamedResourcesAsync<Move>())
        {
            var API_moves = await apiclient.GetResourceAsync(m);
            Moves.Add(new Attack
            {
                ID = API_moves.Name,
                Name = API_moves.Names.FirstOrDefault(n => n.Language.Name == "en").Name,
                Power = API_moves.Power.Value,
                PP = API_moves.Pp.Value,
                DamageClass = DamageClasses.FirstOrDefault(d => d.Name == API_moves.DamageClass.Name)
            });
        }

        var Abilities = new List<Ability>();

        await foreach (var a in apiclient.GetAllNamedResourcesAsync<PokeApiNet.Ability>())
        {
            var API_abilities = await apiclient.GetResourceAsync(a);
            Abilities.Add(new Ability
            {
                ID = API_abilities.Name,
                Name = API_abilities.Names.FirstOrDefault(n => n.Language.Name == "en").Name,
            });
        }

        var Pokemon = new List<Pokemon>();

        await foreach (var p in apiclient.GetAllNamedResourcesAsync<PokeApiNet.Pokemon>())
        {
            var API_pokemon = await apiclient.GetResourceAsync(p);
            var API_Species = await apiclient.GetResourceAsync(API_pokemon.Species);
            var API_Form = await apiclient.GetResourceAsync(API_pokemon.Forms[0]);
            var mon = new Pokemon
            {
                ID = API_pokemon.Name,
                Name = API_Species.Names.FirstOrDefault(n => n.Language.Name == "en").Name,
                FormName = API_Form.FormNames.FirstOrDefault(n => n.Language.Name == "en").Name,
                Stats = new StatBlock
                {
                    HP = API_pokemon.Stats[0].BaseStat,
                    Attack = API_pokemon.Stats[1].BaseStat,
                    Defense = API_pokemon.Stats[2].BaseStat,
                    SpecialAttack = API_pokemon.Stats[3].BaseStat,
                    SpecialDefense = API_pokemon.Stats[4].BaseStat,
                    Speed = API_pokemon.Stats[5].BaseStat,
                }
            };

            List<(Ability Ability, bool IsHidden)> AbilitiesMon = [];

            foreach (var a in API_pokemon.Abilities)
            {
                var ability = Abilities.FirstOrDefault(b => b.ID == a.Ability.Name);
                AbilitiesMon.Add((ability, a.IsHidden));
            }

            mon.Abilities = AbilitiesMon;

            List<PokemonAttack> Learnset = [];
            foreach (var m in API_pokemon.Moves)
            {
                foreach (var vg in m.VersionGroupDetails)
                {
                    var how = vg.MoveLearnMethod.Name;
                    var move = Moves.FirstOrDefault(om => om.ID == m.Move.Name);
                    string details = "";
                    if (how == "level-up")
                        details = vg.LevelLearnedAt.ToString();

                    Learnset.Add(new PokemonAttack
                    {
                        Attack = move,
                        Trigger = how,
                        TriggerDetails = details
                    });
                }
            }

            mon.Learnset = Learnset;
            Pokemon.Add(mon);
        }

        return new Database(Pokemon, Attacks, Abilities, Types, DamageClasses, new List<Effect>());
    }
}