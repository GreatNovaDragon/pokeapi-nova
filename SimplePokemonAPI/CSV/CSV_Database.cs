using System.Globalization;
using CsvHelper;

namespace SimplePokemonAPI.CSV;

public class CsvDatabase
{
    readonly string _dbPath;
    readonly string _pokemonPath;
    readonly string _pokemonAbilityPath;
    readonly string _abilityPath;
    readonly string _effectPath;
    readonly string _typePath;
    readonly string _damageRelationsPath;
    readonly string _damageClassPath;
    readonly string _attackPath;
    readonly string _learnsetPath;

    
    public List<Pokemon> Pokemon { get; set; }
    public List<PokemonAbility> PokemonAbility { get; set; }
    public List<Ability> Abilities { get; set; }
    public List<Effect> Effects { get; set; }
    public List<ElementalType> Types { get; set; }
    public List<DamageRelations> DamageRelations { get; set; }
    public List<DamageClass> DamageClasses { get; set; }
    public List<Attack> Attacks { get; set; }
    public List<PokemonAttack> Learnsets { get; set; }
    public CsvDatabase(): this("./database")
    {
        Console.WriteLine("Creating DB at ./database");
    }


    public CsvDatabase(string DBPath)
    {
        this._dbPath = DBPath;
        _pokemonPath = Path.Combine(DBPath, "pokemon.csv");
        _pokemonAbilityPath = "pokemon_ability.csv";
        _abilityPath = "abilityset.csv";
        _effectPath = "effect.csv";
        _typePath = "type.csv";
        _damageRelationsPath = "damage_relations.csv";
        _damageClassPath = "damage_class.csv";
        _attackPath = "attack.csv";
        _learnsetPath = "learnset.csv";

        this.Pokemon = [];
        this.PokemonAbility = [];
        this.Abilities = [];
        this.Effects = [];
        this.Types = [];
        this.DamageRelations = [];
        this.DamageClasses = [];
        this.Attacks = [];
        this.Learnsets = [];
        
        if (!Directory.Exists(DBPath))
        {
            PrepareEmptyDatabase();
            return;
        }

        throw new NotImplementedException();
        //TODO: Read CSV
    }


    public void PrepareEmptyDatabase()
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

        using (var writer = new StreamWriter( _effectPath))
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