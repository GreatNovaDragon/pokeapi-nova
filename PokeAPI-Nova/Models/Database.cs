using PokeApiNet;
using SimplePokemonAPI.Serializers;
using Type = PokeApiNet.Type;

namespace SimplePokemonAPI.Models;

public class Database(
    List<Pokemon> pokemon,
    List<Move> moves,
    List<Ability> abilities,
    List<ElementalType> types,
    List<DamageClass> damageClasses,
    List<Effect> moveeffects,
    List<Effect> abilityeffects,
    List<VersionGroup> versiongroups,
    List<Version> versions)
{
    public Database() : this([], [], [], [], [], [], [], [], [])
    {
    }


    public List<Pokemon> Pokemon { get; set; } = pokemon;
    public List<Move> Moves { get; set; } = moves;
    public List<Ability> Abilities { get; set; } = abilities;
    public List<ElementalType> Types { get; set; } = types;
    public List<DamageClass> DamageClasses { get; set; } = damageClasses;
    public List<Effect> MoveEffects { get; set; } = moveeffects;
    public List<Effect> AbilityEffects { get; set; } = abilityeffects;

    public List<Version> Versions { get; set; } = versions;

    public List<VersionGroup> VersionGroups { get; set; } = versiongroups;

    public Database GetDatabaseFromSerializer(Serializer serializer)
    {
        var VersionGroups = new List<VersionGroup>();

        foreach (var vg in serializer.VersionGroups)
            VersionGroups.Add(new VersionGroup
            {
                ID = vg.ID,
                Order = vg.Order
            });

        var Versions = new List<Version>();

        foreach (var ver in serializer.Versions)
            Versions.Add(new Version
            {
                ID = ver.ID,
                InVersionGroup = VersionGroups.FirstOrDefault(vg => vg.ID == ver.InVersionGroupID),
                Name = ver.Name
            });

        var Types = serializer.Types.Select(t => new ElementalType
            {
                ID = t.ID, Name = t.Name, DamageRelations = [],
                IntroducedIn = VersionGroups.FirstOrDefault(vg => vg.ID == t.IntroducedInVersionGroupID)
            })
            .ToList();

        foreach (var type in Types)
        foreach (var dr in serializer.DamageRelations.Where(e => e.DefenderID == type.ID))
            type.DamageRelations.Add((Types.FirstOrDefault(e => e.ID == dr.DefenderID), dr.ProzentualMultiplier)!);

        var DamageClasses = serializer.DamageClasses.Select(d => new DamageClass { ID = d.ID, Name = d.Name }).ToList();

        var MoveEffects = serializer.MoveEffects.Select(effect => new Effect
        {
            ID = effect.ID, Description = effect.Description
        }).ToList();

        var Moves = serializer.Moves.Select(attack => new Move
        {
            ID = attack.ID, Name = attack.Name, Effect = MoveEffects.FirstOrDefault(e => e.ID == attack.EffectID),
            Power = attack.Power, PP = attack.PP, Accuracy = attack.Accuracy, Priority = attack.Priority,
            EffectChance = attack.EffectChance,
            DamageClass = DamageClasses.FirstOrDefault(dc => dc.ID == attack.DamageClassID),
            IntroducedIn = VersionGroups.FirstOrDefault(vg => vg.ID == attack.IntroducedInVersionGroupID),
            Type = Types.FirstOrDefault(t => t.ID == attack.TypeID)
        }).ToList();

        var AbilityEffects = serializer.AbilityEffects.Select(effect => new Effect
        {
            ID = effect.ID, Description = effect.Description
        }).ToList();


        var Abilities = serializer.Abilities.Select(ability => new Ability
        {
            ID = ability.ID, Name = ability.Name, Effect = AbilityEffects.FirstOrDefault(e => e.ID == ability.EffectID),
            IntroducedIn = VersionGroups.FirstOrDefault(vg => vg.ID == ability.IntroducedInVersionGroupID)
        }).ToList();


        var Pokemon = new List<Pokemon>();

        foreach (var pkmn in serializer.Pokemon)
        {
            var Learnset = new List<PokemonAttack>();
            foreach (var ls in serializer.Learnsets.FindAll(ls => ls.PokemonID == pkmn.ID))
                Learnset.Add(new PokemonAttack
                {
                    Move = this.Moves.FirstOrDefault(e => e.ID == ls.AttackID)!,
                    Trigger = ls.Trigger,
                    TriggerDetails = ls.TriggerDetails
                });

            List<(Ability Ability, bool isHidden)> AbilityPokemon = serializer.PokemonAbility
                .FindAll(a => a.PokemonID == pkmn.ID).Select(a =>
                    (Abilities.FirstOrDefault(an => an.ID == a.AbilityID), isHidden: a.IsHidden)).ToList()!;


            Pokemon.Add(new Pokemon
            {
                ID = pkmn.ID,
                Name = pkmn.Name,
                FormName = pkmn.FormName,
                Abilities = AbilityPokemon,
                Learnset = Learnset,
                PrimaryType = types.FirstOrDefault(t => t.ID == pkmn.PrimaryTypeID),
                SecundaryType = types.FirstOrDefault(t => t.ID == pkmn.SecundaryTypeID),
                IntroducedIn = VersionGroups.FirstOrDefault(vg => vg.ID == pkmn.IntroducedInVersionGroupID),
                Stats = new StatBlock
                {
                    Attack = pkmn.Stats.Attack, SpecialAttack = pkmn.Stats.SpecialAttack, Defense = pkmn.Stats.Defense,
                    SpecialDefense = pkmn.Stats.SpecialDefense, Speed = pkmn.Stats.Speed, HP = pkmn.Stats.HP
                }
            });
        }

        foreach (var mon in serializer.VisualOnlyPokemon)
        {
            var basedOn = Pokemon.FirstOrDefault(m => m.ID == mon.basedOnPokemonID);
            Pokemon.Add(new Pokemon
            {
                ID = mon.ID, Name = mon.Name, FormName = mon.FormName, Abilities = basedOn.Abilities,
                Stats = basedOn.Stats, Learnset = basedOn.Learnset,
                IntroducedIn = VersionGroups.FirstOrDefault(vg => vg.ID == mon.IntroducedInVersionGroupID)
            });
        }

        return new Database(Pokemon, Moves, Abilities, Types, DamageClasses, MoveEffects, AbilityEffects,
            versiongroups, versions);
    }

    public async Task<Database> GetDatabaseFromPokeAPIWithoutEffects(string lang)
    {
        var apiclient = new PokeApiClient();

        var VersionGroups = new List<VersionGroup>();
        var Versions = new List<Version>();


        await foreach (var vg in apiclient.GetAllNamedResourcesAsync<PokeApiNet.VersionGroup>())
        {
            var ApiVersionGroup = await apiclient.GetResourceAsync(vg);
            VersionGroups.Add(new VersionGroup
            {
                ID = ApiVersionGroup.Name,
                Order = ApiVersionGroup.Order
            });
        }

        Console.WriteLine("Done with VersionGroups");


        await foreach (var ver in apiclient.GetAllNamedResourcesAsync<PokeApiNet.Version>())
        {
            var ApiGameVersion = await apiclient.GetResourceAsync(ver);
            var name = ApiGameVersion.Names.Where(l => l.Name == lang).FirstOrDefault();
            Versions.Add(new Version
            {
                ID = ApiGameVersion.Name, Name = name == null ? "" : name.Name,
                InVersionGroup = VersionGroups.FirstOrDefault(vg => vg.ID == ApiGameVersion.VersionGroup.Name)
            });
        }

        Console.WriteLine("Done with Versions");


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

        Console.WriteLine("Done with Types");


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

        Console.WriteLine("Done with TypeRelations");


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

        Console.WriteLine("Done with DamageClassess");

        var Moves = new List<Move>();

        await foreach (var m in apiclient.GetAllNamedResourcesAsync<PokeApiNet.Move>())
        {
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

        Console.WriteLine("Done with Moves");


        var Abilities = new List<Ability>();

        await foreach (var a in apiclient.GetAllNamedResourcesAsync<PokeApiNet.Ability>())
        {
            var API_abilities = await apiclient.GetResourceAsync(a);
            var Name = API_abilities.Names.FirstOrDefault(n => n.Language.Name == lang);
            Abilities.Add(new Ability
            {
                ID = API_abilities.Name,
                Name = Name == null ? "" : Name.Name,
                Effect = null
            });
        }

        Console.WriteLine("Done with Abilities");

        var Pokemon = new List<Pokemon>();

        var i = 0;
        await foreach (var p in apiclient.GetAllNamedResourcesAsync<PokemonForm>())
        {
            i++;
            Console.WriteLine($"{p.Name} {i}");
            var API_Form = await apiclient.GetResourceAsync(p);
            var API_pokemon = await apiclient.GetResourceAsync(API_Form.Pokemon);
            var API_Species = await apiclient.GetResourceAsync(API_pokemon.Species);
            var Name = API_Species.Names.FirstOrDefault(n => n.Language.Name == lang);
            if (Name == null) Name = API_Species.Names.FirstOrDefault(n => n.Language.Name == lang);

            var FormName = API_Form.FormNames.FirstOrDefault(n => n.Language.Name == lang);
            var PrimaryType = Types.FirstOrDefault(tp => tp.ID == API_pokemon.Types[0].Type.Name);
            var SecundaryType = API_pokemon.Types.Count > 1
                ? Types.FirstOrDefault(tp => tp.ID == API_pokemon.Types[1].Type.Name)
                : null;


            var mon = new Pokemon
            {
                ID = API_Form.Name,
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
                },
                PrimaryType = PrimaryType,
                SecundaryType = SecundaryType
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
                var Versiongroup = VersionGroups.FirstOrDefault(verg => verg.ID == vg.VersionGroup.Name);
                Console.WriteLine($"{Versiongroup.ID}");
                var how = vg.MoveLearnMethod.Name;
                var move = Moves.FirstOrDefault(om => om.ID == m.Move.Name);
                var details = "";
                if (how == "level-up")
                    details = vg.LevelLearnedAt.ToString();

                Learnset.Add(new PokemonAttack
                {
                    Move = move,
                    Trigger = how,
                    TriggerDetails = details,
                    AppliesTo = Versiongroup
                });
            }

            mon.Learnset = Learnset;
            Pokemon.Add(mon);
        }

        Console.WriteLine("Done with Mons");


        GC.Collect();
        GC.WaitForPendingFinalizers();


        return new Database(Pokemon, Moves, Abilities, Types, DamageClasses, new List<Effect>(),
            new List<Effect>(), VersionGroups, Versions);
    }
}