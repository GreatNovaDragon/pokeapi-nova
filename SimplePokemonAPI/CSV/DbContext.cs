using System.Globalization;
using CsvHelper;
using SimplePokemonAPI.Models;

namespace SimplePokemonAPI.CSV;

public class DbContext
{
    public static void PrepareEmptyDatabase()
    {
        using (var writer = new StreamWriter("pokemon.csv"))
        using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
        {
            csv.WriteHeader<Pokemon>();
        }

        using (var writer = new StreamWriter("pokemon_ability.csv"))
        using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
        {
            csv.WriteHeader<PokemonAbility>();
        }


        using (var writer = new StreamWriter("ability.csv"))
        using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
        {
            csv.WriteHeader<Ability>();
        }

        using (var writer = new StreamWriter("ability.csv"))
        using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
        {
            csv.WriteHeader<Ability>();
        }

        using (var writer = new StreamWriter("effect.csv"))
        using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
        {
            csv.WriteHeader<Effect>();
        }

        using (var writer = new StreamWriter("type.csv"))
        using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
        {
            csv.WriteHeader<ElementalType>();
        }

        using (var writer = new StreamWriter("damage_relations.csv"))
        using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
        {
            csv.WriteHeader<DamageRelations>();
        }

        using (var writer = new StreamWriter("damage_class.csv"))
        using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
        {
            csv.WriteHeader<DamageClass>();
        }

        using (var writer = new StreamWriter("attack.csv"))
        using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
        {
            csv.WriteHeader<Attack>();
        }

        using (var writer = new StreamWriter("learnset.csv"))
        using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
        {
            csv.WriteHeader<PokemonAttack>();
        }
    }

    
        public static void PrepareEmptyDatabase()
    {
        using (var writer = new StreamWriter("pokemon.csv"))
        using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
        {
            csv.WriteHeader<Pokemon>();
        }

        using (var writer = new StreamWriter("pokemon_ability.csv"))
        using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
        {
            csv.WriteHeader<PokemonAbility>();
        }


        using (var writer = new StreamWriter("ability.csv"))
        using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
        {
            csv.WriteHeader<Ability>();
        }

        using (var writer = new StreamWriter("ability.csv"))
        using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
        {
            csv.WriteHeader<Ability>();
        }

        using (var writer = new StreamWriter("effect.csv"))
        using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
        {
            csv.WriteHeader<Effect>();
        }

        using (var writer = new StreamWriter("type.csv"))
        using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
        {
            csv.WriteHeader<ElementalType>();
        }

        using (var writer = new StreamWriter("damage_relations.csv"))
        using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
        {
            csv.WriteHeader<DamageRelations>();
        }

        using (var writer = new StreamWriter("damage_class.csv"))
        using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
        {
            csv.WriteHeader<DamageClass>();
        }

        using (var writer = new StreamWriter("attack.csv"))
        using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
        {
            csv.WriteHeader<Attack>();
        }

        using (var writer = new StreamWriter("learnset.csv"))
        using (var csv = new CsvWriter(writer, CultureInfo.InvariantCulture))
        {
            csv.WriteHeader<PokemonAttack>();
        }
    }
    public static Database ReadDatabase()
    {
        Database DB = new Database();

        List<Models.Effect> effects = [];
        using (var reader = new StreamReader("effect.csv"))
        using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
        {
            csv.Read();
            csv.ReadHeader();
            while (csv.Read())
            {
                var record = csv.GetRecord<CSV.Effect>();
                Models.Effect effect = new Models.Effect();
                effect.ID = record.ID;
                effect.Description = record.Description;
                effect.Type = (Models.EffectType)Enum.Parse(typeof(Models.EffectType), record.Type);
                effects.Add(effect);
            }
        }

        DB.Effects = effects;

        List<Models.Ability> abilities = [];
        using (var reader = new StreamReader("ability.csv"))
        using (var csv = new CsvReader(reader, CultureInfo.InvariantCulture))
        {
            csv.Read();
            csv.ReadHeader();
            while (csv.Read())
            {
                var record = csv.GetRecord<CSV.Ability>();
                Models.Ability ability = new Models.Ability();
                ability.ID = record.ID;
                ability.Name = record.Name;
                ability.Effect = DB.Effects.FirstOrDefault(e => e.ID == record.EffectID) ??
                                 throw new InvalidOperationException();
                abilities.Add(ability);
            }
        }

        DB.Abilities = abilities;
        return DB;
    }
}