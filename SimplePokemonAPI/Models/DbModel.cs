using PokeApiNet;
using SimplePokemonAPI.CSV;
using Type = PokeApiNet.Type;

namespace SimplePokemonAPI.Models;

public class Database(
    List<Pokemon> pokemon,
    List<Attack> attacks,
    List<Ability> abilities,
    List<ElementalType> types,
    List<DamageClass> damageClasses,
    List<Effect> effects)
{
    public Database() : this([], [], [], [], [], [])
    {
    }


    public List<Pokemon> Pokemon { get; set; } = pokemon;
    public List<Attack> Attacks { get; set; } = attacks;
    public List<Ability> Abilities { get; set; } = abilities;
    public List<ElementalType> Types { get; set; } = types;
    public List<DamageClass> DamageClasses { get; set; } = damageClasses;
    public List<Effect> Effects { get; set; } = effects;

    public Database GetDatabaseFromCSV(string path = "./database")
    {
        var csvdb = new CsvDatabase(path);

        var Types = csvdb.Types.Select(t => new ElementalType { ID = t.ID, Name = t.Name, DamageRelations = [] })
            .ToList();

        foreach (var type in Types)
        {
            foreach (var dr in csvdb.DamageRelations.Where(e => e.DefenderID == type.ID))
            {
                type.DamageRelations.Add((Types.FirstOrDefault(e => e.ID == dr.DefenderID), dr.ProzentualMultiplier)!);
            }
        }

        var DamageClasses = csvdb.DamageClasses.Select(d => new DamageClass { ID = d.ID, Name = d.Name, }).ToList();

        var Effects = csvdb.Effects.Select(effect => new Effect
        {
            ID = effect.ID, Description = effect.Description,
            Type = (EffectType)Enum.Parse(typeof(EffectType), effect.Type),
        }).ToList();

        var Moves = csvdb.Attacks.Select(attack => new Attack
        {
            ID = attack.ID, Name = attack.Name, Effect = Effects.FirstOrDefault(e => e.ID == attack.EffectID),
            Power = attack.Power, PP = attack.PP,
            DamageClass = DamageClasses.FirstOrDefault(dc => dc.ID == attack.DamageClassID)
        }).ToList();


        var Abilities = csvdb.Abilities.Select(ability => new Ability
        {
            ID = ability.ID, Name = ability.Name, Effect = Effects.FirstOrDefault(e => e.ID == ability.EffectID),
        }).ToList();


        var Pokemon = new List<Pokemon>();

        foreach (var pkmn in csvdb.Pokemon)
        {
            var Learnset = new List<PokemonAttack>();
            foreach (var ls in csvdb.Learnsets.FindAll(ls => ls.PokemonID == pkmn.ID))
            {
                Learnset.Add(new PokemonAttack
                {
                    Attack = Attacks.FirstOrDefault(e => e.ID == ls.AttackID)!,
                    Trigger = ls.Trigger,
                    TriggerDetails = ls.TriggerDetails
                });
            }

            List<(Ability Ability, bool isHidden)> AbilityPokemon = csvdb.PokemonAbility
                .FindAll(a => a.PokemonID == pkmn.ID).Select(a =>
                    (Abilities.FirstOrDefault(an => an.ID == a.AbilityID), isHidden: a.IsHidden)).ToList()!;


            Pokemon.Add(new Pokemon
            {
                ID = pkmn.ID,
                Name = pkmn.Name,
                FormName = pkmn.FormName,
                Abilities = AbilityPokemon,
                Learnset = Learnset,
                Stats = new StatBlock
                {
                    Attack = pkmn.Stats.Attack, SpecialAttack = pkmn.Stats.SpecialAttack, Defense = pkmn.Stats.Defense,
                    SpecialDefense = pkmn.Stats.SpecialDefense, Speed = pkmn.Stats.Speed, HP = pkmn.Stats.HP
                }
            });
        }

        return new Database(Pokemon, Moves, Abilities, Types, DamageClasses, Effects);
    }

    public async Task<Database> GetDatabaseFromPokeAPIWithoutEffects()
    {
        var apiclient = new PokeApiClient();

        var Types = new List<ElementalType>();

        await foreach (var typeRessource in apiclient.GetAllNamedResourcesAsync<Type>())
        {
            var ApiType = await apiclient.GetResourceAsync(typeRessource);
            Types.Add(
                new ElementalType
                {
                    ID = ApiType.Name,
                    Name = ApiType.Names.FirstOrDefault(n => n.Language.Name == "en").Name,
                    DamageRelations = []
                }
            );
        }

        foreach (var Type in Types)
        {
            var ApiType = await apiclient.GetResourceAsync<Type>(Type.ID);
            foreach (var dt in ApiType.DamageRelations.DoubleDamageTo)
                Type.DamageRelations.Add((Types.FirstOrDefault(t => t.ID == dt.Name), 200));

            foreach (var ht in ApiType.DamageRelations.HalfDamageTo)
            {
                var type = Types.FirstOrDefault(t => t.ID == ht.Name);
                Type.DamageRelations.Add((type, 50));
            }

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
            Console.WriteLine($"{m.Name}  {m.Url}");
            var API_moves = await apiclient.GetResourceAsync(m);

            var ID = API_moves.Name;
            var Name = API_moves.Names.FirstOrDefault(n => n.Language.Name == "en").Name;
            var Power = API_moves.Power;
            var PP = API_moves.Pp;
            var DamageClass = DamageClasses.FirstOrDefault(d => d.Name == API_moves.DamageClass.Name);
            Effect? Effect = null;

            Moves.Add(new Attack
            {
                ID = ID,
                Name = Name,
                Power = Power,
                PP = PP,
                DamageClass = DamageClass,
                Effect = Effect
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
                Effect = null
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
                    Speed = API_pokemon.Stats[5].BaseStat
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
            foreach (var vg in m.VersionGroupDetails)
            {
                var how = vg.MoveLearnMethod.Name;
                var move = Moves.FirstOrDefault(om => om.ID == m.Move.Name);
                var details = "";
                if (how == "level-up")
                    details = vg.LevelLearnedAt.ToString();

                Learnset.Add(new PokemonAttack
                {
                    Attack = move,
                    Trigger = how,
                    TriggerDetails = details
                });
            }

            mon.Learnset = Learnset;
            Pokemon.Add(mon);
        }

        return new Database(Pokemon, Moves, Abilities, Types, DamageClasses, new List<Effect>());
    }
}