using System.Globalization;
using CsvHelper;

namespace SimplePokemonAPI.FileModels;

public class CSV : FileModel
{
    private readonly string _abilityPath;
    private readonly string _attackPath;
    private readonly string _damageClassPath;
    private readonly string _damageRelationsPath;
    private readonly string _dbPath;
    private readonly string _effectPath;
    private readonly string _learnsetPath;
    private readonly string _pokemonAbilityPath;
    private readonly string _pokemonPath;
    private readonly string _typePath;

    public CSV() : this("./database")
    {
        Console.WriteLine("Creating DB at ./database");
    }


    private CSV(string dbPath)
    {
        _dbPath = dbPath;
        _pokemonPath = Path.Combine(dbPath, "pokemon.csv");
        _pokemonAbilityPath = Path.Combine(dbPath, "pokemon_ability.csv");
        _abilityPath = Path.Combine(dbPath, "abilityset.csv");
        _effectPath = Path.Combine(dbPath, "effect.csv");
        _typePath = Path.Combine(dbPath, "type.csv");
        _damageRelationsPath = Path.Combine(dbPath, "damage_relations.csv");
        _damageClassPath = Path.Combine(dbPath, "damage_class.csv");
        _attackPath = Path.Combine(dbPath, "attack.csv");
        _learnsetPath = Path.Combine(dbPath, "learnset.csv");
    }

    public override void Read()
    {
        using (var writer = new StreamReader(_pokemonPath))
        using (var csv = new CsvReader(writer, CultureInfo.InvariantCulture))
        {
            Pokemon = csv.GetRecords<Pokemon>().ToList();
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

        using (var writer = new StreamReader(_effectPath))
        using (var csv = new CsvReader(writer, CultureInfo.InvariantCulture))
        {
            Effects = csv.GetRecords<Effect>().ToList();
        }

        using (var writer = new StreamReader(_typePath))
        using (var csv = new CsvReader(writer, CultureInfo.InvariantCulture))
        {
            Types = csv.GetRecords<ElementalType>().ToList();
        }

        using (var writer = new StreamReader(_damageRelationsPath))
        using (var csv = new CsvReader(writer, CultureInfo.InvariantCulture))
        {
            DamageRelations = csv.GetRecords<DamageRelations>().ToList();
        }

        using (var writer = new StreamReader(_damageClassPath))
        using (var csv = new CsvReader(writer, CultureInfo.InvariantCulture))
        {
            DamageClasses = csv.GetRecords<DamageClass>().ToList();
        }

        using (var writer = new StreamReader(_attackPath))
        using (var csv = new CsvReader(writer, CultureInfo.InvariantCulture))
        {
            Attacks = csv.GetRecords<Attack>().ToList();
        }

        using (var writer = new StreamReader(_learnsetPath))
        using (var csv = new CsvReader(writer, CultureInfo.InvariantCulture))
        {
            Learnsets = csv.GetRecords<PokemonAttack>().ToList();
        }
    }

    public override void Write()
    {
        Directory.CreateDirectory(_dbPath);

        using (var writer = new StreamWriter(_pokemonPath))
        using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
        {
            csv.WriteRecords(Pokemon);
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

        using (var writer = new StreamWriter(_effectPath))
        using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
        {
            csv.WriteRecords(Effects);
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

        using (var writer = new StreamWriter(_attackPath))
        using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
        {
            csv.WriteRecords(Attacks);
        }

        using (var writer = new StreamWriter(_learnsetPath))
        using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
        {
            csv.WriteRecords(Learnsets);
        }
    }

    public override void PrepareEmptyDatabase()
    {
        Write();
    }
}