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

    public CSV(bool read = false) : this("./database", read)
    {
        Console.WriteLine("Creating DB at ./database");
    }


    private CSV(string dbPath, bool read = false)
    {
        _dbPath = dbPath;
        _pokemonPath = Path.Combine(dbPath, "pokemon.csv");
        _pokemonAbilityPath = Path.Combine(dbPath,"pokemon_ability.csv");
        _abilityPath = Path.Combine(dbPath,"abilityset.csv");
        _effectPath = Path.Combine(dbPath,"effect.csv");
        _typePath = Path.Combine(dbPath,"type.csv");
        _damageRelationsPath = Path.Combine(dbPath,"damage_relations.csv");
        _damageClassPath = Path.Combine(dbPath,"damage_class.csv");
        _attackPath = Path.Combine(dbPath,"attack.csv");
        _learnsetPath = Path.Combine(dbPath,"learnset.csv");

        if (read)
        {
            throw new NotImplementedException();
            //TODO:Read CSV   
        }
        else
        {
            if (!Directory.Exists(dbPath))
            {
                PrepareEmptyDatabase();
            }
        }
    }


    private void PrepareEmptyDatabase()
    {
        Directory.CreateDirectory(_dbPath);

        using (var writer = new StreamWriter(_pokemonPath))
        using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
        {
            csv.WriteHeader<Pokemon>();
        }

        using (var writer = new StreamWriter(_pokemonAbilityPath))
        using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
        {
            csv.WriteHeader<PokemonAbility>();
        }


        using (var writer = new StreamWriter(_abilityPath))
        using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
        {
            csv.WriteHeader<Ability>();
        }

        using (var writer = new StreamWriter(_effectPath))
        using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
        {
            csv.WriteHeader<Effect>();
        }

        using (var writer = new StreamWriter(_typePath))
        using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
        {
            csv.WriteHeader<ElementalType>();
        }

        using (var writer = new StreamWriter(_damageRelationsPath))
        using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
        {
            csv.WriteHeader<DamageRelations>();
        }

        using (var writer = new StreamWriter(_damageClassPath))
        using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
        {
            csv.WriteHeader<DamageClass>();
        }

        using (var writer = new StreamWriter(_attackPath))
        using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
        {
            csv.WriteHeader<Attack>();
        }

        using (var writer = new StreamWriter(_learnsetPath))
        using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
        {
            csv.WriteHeader<PokemonAttack>();
        }
    }
}