using System.Globalization;
using CsvHelper;

namespace SimplePokemonAPI.Serializers;

public class CSV_Serializer(string dbPath) : Serializer
{
    private readonly string _abilityEffectPath = Path.Combine(dbPath, "moves/effect.csv");
    private readonly string _abilityPath = Path.Combine(dbPath, "abilities/ability.csv");
    private readonly string _damageClassPath = Path.Combine(dbPath, "moves/damage_class.csv");
    private readonly string _damageRelationsPath = Path.Combine(dbPath, "types/damage_relations.csv");

    private readonly string _learnsetPath = Path.Combine(dbPath, "pokemon/learnset.csv");
    private readonly string _moveEffectPath = Path.Combine(dbPath, "abilities/effect.csv");
    private readonly string _movePath = Path.Combine(dbPath, "moves/moves.csv");
    private readonly string _pokemonAbilityPath = Path.Combine(dbPath, "pokemon/pokemon_ability.csv");
    private readonly string _pokemonPath = Path.Combine(dbPath, "pokemon/pokemon.csv");

    private readonly string _typePath = Path.Combine(dbPath, "types/type.csv");
    private readonly string _visualonlypokemonPath = Path.Combine(dbPath, "pokemon/visualonly.csv");

    public CSV_Serializer() : this("./database/")
    {
    }


    public override void Read()
    {
        using (var writer = new StreamReader(_pokemonPath))
        using (var csv = new CsvReader(writer, CultureInfo.InvariantCulture))
        {
            Pokemon = csv.GetRecords<Pokemon>().ToList();
        }

        using (var writer = new StreamReader(_visualonlypokemonPath))
        using (var csv = new CsvReader(writer, CultureInfo.InvariantCulture))
        {
            VisualOnlyPokemon = csv.GetRecords<VisualOnlyPokemon>().ToList();
        }

        using (var writer = new StreamReader(_pokemonAbilityPath))
        using (var csv = new CsvReader(writer, CultureInfo.InvariantCulture))
        {
            PokemonAbility = csv.GetRecords<PokemonAbility>().ToList();
        }


        using (var writer = new StreamReader(_abilityPath))
        using (var csv = new CsvReader(writer, CultureInfo.InvariantCulture))
        {
            Abilities = csv.GetRecords<Ability>().ToList();
        }

        using (var writer = new StreamReader(_abilityEffectPath))
        using (var csv = new CsvReader(writer, CultureInfo.InvariantCulture))
        {
            AbilityEffects = csv.GetRecords<Effect>().ToList();
        }

        using (var writer = new StreamReader(_moveEffectPath))
        using (var csv = new CsvReader(writer, CultureInfo.InvariantCulture))
        {
            MoveEffects = csv.GetRecords<Effect>().ToList();
        }

        using (var writer = new StreamReader(_typePath))
        using (var csv = new CsvReader(writer, CultureInfo.InvariantCulture))
        {
            Types = csv.GetRecords<ElementalType>().ToList();
        }

        using (var writer = new StreamReader(_damageRelationsPath))
        using (var csv = new CsvReader(writer, CultureInfo.InvariantCulture))
        {
            DamageRelations = csv.GetRecords<DamageRelation>().ToList();
        }

        using (var writer = new StreamReader(_damageClassPath))
        using (var csv = new CsvReader(writer, CultureInfo.InvariantCulture))
        {
            DamageClasses = csv.GetRecords<DamageClass>().ToList();
        }

        using (var writer = new StreamReader(_movePath))
        using (var csv = new CsvReader(writer, CultureInfo.InvariantCulture))
        {
            Moves = csv.GetRecords<Move>().ToList();
        }

        using (var writer = new StreamReader(_learnsetPath))
        using (var csv = new CsvReader(writer, CultureInfo.InvariantCulture))
        {
            Learnsets = csv.GetRecords<PokemonAttack>().ToList();
        }
    }

    public override void Write()
    {
        Directory.CreateDirectory(dbPath);
        Directory.CreateDirectory(Path.Combine(dbPath, "moves"));
        Directory.CreateDirectory(Path.Combine(dbPath, "abilities"));
        Directory.CreateDirectory(Path.Combine(dbPath, "pokemon"));
        Directory.CreateDirectory(Path.Combine(dbPath, "types"));

        using (var writer = new StreamWriter(_pokemonPath))
        using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
        {
            csv.WriteRecords(Pokemon);
        }

        using (var writer = new StreamWriter(_visualonlypokemonPath))
        using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
        {
            csv.WriteRecords(VisualOnlyPokemon);
        }

        using (var writer = new StreamWriter(_pokemonAbilityPath))
        using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
        {
            csv.WriteRecords(PokemonAbility);
        }


        using (var writer = new StreamWriter(_abilityPath))
        using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
        {
            csv.WriteRecords(Abilities);
        }

        using (var writer = new StreamWriter(_moveEffectPath))
        using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
        {
            csv.WriteRecords(MoveEffects);
        }

        using (var writer = new StreamWriter(_abilityEffectPath))
        using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
        {
            csv.WriteRecords(AbilityEffects);
        }

        using (var writer = new StreamWriter(_typePath))
        using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
        {
            csv.WriteRecords(Types);
        }

        using (var writer = new StreamWriter(_damageRelationsPath))
        using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
        {
            csv.WriteRecords(DamageRelations);
        }

        using (var writer = new StreamWriter(_damageClassPath))
        using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
        {
            csv.WriteRecords(DamageClasses);
        }

        using (var writer = new StreamWriter(_movePath))
        using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
        {
            csv.WriteRecords(Moves);
        }

        using (var writer = new StreamWriter(_learnsetPath))
        using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
        {
            csv.WriteRecords(Learnsets);
        }
    }

    public override void PrepareEmpty()
    {
        Write();
    }
}