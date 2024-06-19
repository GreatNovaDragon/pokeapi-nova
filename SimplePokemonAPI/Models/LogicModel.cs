using PokeApiNet;
using SimplePokemonAPI.Serializers;
using Type = PokeApiNet.Type;

namespace SimplePokemonAPI.Models;

public class Knowledgebase(
    List<Pokemon> pokemon,
    List<Move> moves,
    List<Ability> abilities,
    List<ElementalType> types,
    List<DamageClass> damageClasses,
    List<Effect> moveeffects,
    List<Effect> abilityeffects)
{
    public Knowledgebase() : this([], [], [], [], [], [], [])
    {
    }


    public List<Pokemon> Pokemon { get; set; } = pokemon;
    public List<Move> Moves { get; set; } = moves;
    public List<Ability> Abilities { get; set; } = abilities;
    public List<ElementalType> Types { get; set; } = types;
    public List<DamageClass> DamageClasses { get; set; } = damageClasses;
    public List<Effect> MoveEffects { get; set; } = moveeffects;
    public List<Effect> AbilityEffects { get; set; } = abilityeffects;

    public Knowledgebase GetKnowledebaseFromDatabase(Serializer filemodel)
    {
        var Types = filemodel.Types.Select(t => new ElementalType { ID = t.ID, Name = t.Name, DamageRelations = [] })
            .ToList();

        foreach (var type in Types)
        foreach (var dr in filemodel.DamageRelations.Where(e => e.DefenderID == type.ID))
            type.DamageRelations.Add((Types.FirstOrDefault(e => e.ID == dr.DefenderID), dr.ProzentualMultiplier)!);

        var DamageClasses = filemodel.DamageClasses.Select(d => new DamageClass { ID = d.ID, Name = d.Name }).ToList();

        var MoveEffects = filemodel.MoveEffects.Select(effect => new Effect
        {
            ID = effect.ID, Description = effect.Description
        }).ToList();

        var Moves = filemodel.Moves.Select(attack => new Move
        {
            ID = attack.ID, Name = attack.Name, Effect = MoveEffects.FirstOrDefault(e => e.ID == attack.EffectID),
            Power = attack.Power, PP = attack.PP, Accuracy = attack.Accuracy, Priority = attack.Priority,
            EffectChance = attack.EffectChance,
            DamageClass = DamageClasses.FirstOrDefault(dc => dc.ID == attack.DamageClassID)
        }).ToList();

        var AbilityEffects = filemodel.AbilityEffects.Select(effect => new Effect
        {
            ID = effect.ID, Description = effect.Description
        }).ToList();


        var Abilities = filemodel.Abilities.Select(ability => new Ability
        {
            ID = ability.ID, Name = ability.Name, Effect = AbilityEffects.FirstOrDefault(e => e.ID == ability.EffectID)
        }).ToList();


        var Pokemon = new List<Pokemon>();

        foreach (var pkmn in filemodel.Pokemon)
        {
            var Learnset = new List<PokemonAttack>();
            foreach (var ls in filemodel.Learnsets.FindAll(ls => ls.PokemonID == pkmn.ID))
                Learnset.Add(new PokemonAttack
                {
                    Move = this.Moves.FirstOrDefault(e => e.ID == ls.AttackID)!,
                    Trigger = ls.Trigger,
                    TriggerDetails = ls.TriggerDetails
                });

            List<(Ability Ability, bool isHidden)> AbilityPokemon = filemodel.PokemonAbility
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

        foreach (var mon in filemodel.VisualOnlyPokemon)
        {
            var basedOn = Pokemon.FirstOrDefault(m => m.ID == mon.basedOnPokemonID);
            Pokemon.Add(new Pokemon
            {
                ID = mon.ID, Name = mon.Name, FormName = mon.FormName, Abilities = basedOn.Abilities,
                Stats = basedOn.Stats, Learnset = basedOn.Learnset
            });
        }

        return new Knowledgebase(Pokemon, Moves, Abilities, Types, DamageClasses, MoveEffects, AbilityEffects);
    }

    public async Task<Knowledgebase> GetDatabaseFromPokeAPIWithoutEffects(string lang)
    {
        var apiclient = new PokeApiClient();

        var Types = new List<ElementalType>();

        await foreach (var typeRessource in apiclient.GetAllNamedResourcesAsync<Type>())
        {
            var ApiType = await apiclient.GetResourceAsync(typeRessource);
            var Name = ApiType.Names.FirstOrDefault(n => n.Language.Name == lang);
            Types.Add(
                new ElementalType
                {
                    ID = ApiType.Name,
                    Name = Name == null ? "" : Name.Name,
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
                    Name = API_damageclass.Names.FirstOrDefault(n => n.Language.Name == lang).Name
                });
        }

        var Moves = new List<Move>();

        await foreach (var m in apiclient.GetAllNamedResourcesAsync<PokeApiNet.Move>())
        {
            Console.WriteLine($"{m.Name}  {m.Url}");

            var ID = m.Name;
            var Name = m.Name;
            int? Power = 999;
            int? PP = 999;
            var DamageClass = DamageClasses.FirstOrDefault();
            int? Accuracy = 999;
            int? Priority = 999;
            int? EffectChance = 999;

            try
            {
                var API_moves = await apiclient.GetResourceAsync(m);
                Name = API_moves.Names.FirstOrDefault(n => n.Language.Name == lang) == null
                    ? ""
                    : API_moves.Names.FirstOrDefault(n => n.Language.Name == lang).Name;
                Power = API_moves.Power;
                PP = API_moves.Pp;
                DamageClass = DamageClasses.FirstOrDefault(d => d.Name == API_moves.DamageClass.Name);
                Accuracy = API_moves.Accuracy;
                Priority = API_moves.Priority;
                EffectChance = API_moves.EffectChance;
            }
            catch (HttpRequestException)
            {
            }


            Effect? Effect = null;

            Moves.Add(new Move
            {
                ID = ID,
                Name = Name,
                Power = Power,
                PP = PP,
                Accuracy = Accuracy,
                Priority = Priority,
                DamageClass = DamageClass,
                EffectChance = EffectChance,
                Effect = Effect
            });
        }

        var Abilities = new List<Ability>();

        await foreach (var a in apiclient.GetAllNamedResourcesAsync<PokeApiNet.Ability>())
        {
            Console.WriteLine($"{a.Name}  {a.Url}");

            var API_abilities = await apiclient.GetResourceAsync(a);
            var Name = API_abilities.Names.FirstOrDefault(n => n.Language.Name == lang);
            Abilities.Add(new Ability
            {
                ID = API_abilities.Name,
                Name = Name == null ? "" : Name.Name,
                Effect = null
            });
        }

        var Pokemon = new List<Pokemon>();

        await foreach (var p in apiclient.GetAllNamedResourcesAsync<PokeApiNet.Pokemon>())
        {
            Console.WriteLine($"{p.Name}  {p.Url}");

            var API_pokemon = await apiclient.GetResourceAsync(p);
            var API_Species = await apiclient.GetResourceAsync(API_pokemon.Species);
            var API_Form = await apiclient.GetResourceAsync(API_pokemon.Forms[0]);
            var Name = API_Species.Names.FirstOrDefault(n => n.Language.Name == lang);
            var FormName = API_Form.Names.FirstOrDefault(n => n.Language.Name == lang);
            var mon = new Pokemon
            {
                ID = API_pokemon.Name,
                Name = Name == null
                    ? ""
                    : Name.Name,

                FormName = FormName == null
                    ? ""
                    : FormName.Name,
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
                    Move = move,
                    Trigger = how,
                    TriggerDetails = details
                });
            }

            mon.Learnset = Learnset;
            Pokemon.Add(mon);
        }

        return new Knowledgebase(Pokemon, Moves, Abilities, Types, DamageClasses, new List<Effect>(),
            new List<Effect>());
    }
}