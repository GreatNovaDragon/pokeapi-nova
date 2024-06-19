using SimplePokemonAPI.Models;
using Ability = SimplePokemonAPI.Serializers.Ability;
using DamageClass = SimplePokemonAPI.Serializers.DamageClass;
using Effect = SimplePokemonAPI.Serializers.Effect;
using ElementalType = SimplePokemonAPI.Serializers.ElementalType;
using Move = SimplePokemonAPI.Serializers.Move;
using Pokemon = SimplePokemonAPI.Serializers.Pokemon;
using PokemonAttack = SimplePokemonAPI.Serializers.PokemonAttack;
using StatBlock = SimplePokemonAPI.Serializers.StatBlock;

namespace SimplePokemonAPI.Serializers;

public abstract class Serializer
{
    public List<Pokemon> Pokemon { get; set; } = [];
    public List<VisualOnlyPokemon> VisualOnlyPokemon { get; set; } = [];
    public List<PokemonAbility> PokemonAbility { get; set; } = [];
    public List<Ability> Abilities { get; set; } = [];
    public List<Effect> MoveEffects { get; set; } = [];
    public List<Effect> AbilityEffects { get; set; } = [];

    public List<ElementalType> Types { get; set; } = [];
    public List<DamageRelation> DamageRelations { get; set; } = [];
    public List<DamageClass> DamageClasses { get; set; } = [];
    public List<Move> Moves { get; set; } = [];
    public List<PokemonAttack> Learnsets { get; set; } = [];

    public abstract void Read();

    public abstract void Write();

    public abstract void PrepareEmpty();


    public void OverwriteWithDataFromDB(Knowledgebase db)
    {
        foreach (var pokemon in db.Pokemon)
        {
            Pokemon.Add(new Pokemon
            {
                ID = pokemon.ID,
                Name = pokemon.Name,
                FormName = pokemon.FormName,
                Stats = new StatBlock
                {
                    HP = pokemon.Stats.HP,
                    Attack = pokemon.Stats.Attack,
                    Defense = pokemon.Stats.Defense,
                    SpecialAttack = pokemon.Stats.SpecialAttack,
                    SpecialDefense = pokemon.Stats.SpecialDefense,
                    Speed = pokemon.Stats.Speed
                }
            });

            foreach (var ab in pokemon.Abilities)
                PokemonAbility.Add(new PokemonAbility
                {
                    AbilityID = ab.Ability.ID,
                    PokemonID = pokemon.ID,
                    IsHidden = ab.IsHidden
                });

            foreach (var pa in pokemon.Learnset)
                Learnsets.Add(new PokemonAttack
                {
                    AttackID = pa.Move.ID,
                    PokemonID = pokemon.ID,
                    Trigger = pa.Trigger,
                    TriggerDetails = pa.TriggerDetails
                });
        }


        foreach (var ability in db.Abilities)
            Abilities.Add(new Ability
            {
                ID = ability.ID,
                Name = ability.Name,
                EffectID = ability.Effect == null ? "" : ability.Effect.ID
            });


        foreach (var eff in db.MoveEffects) MoveEffects.Add(new Effect { Description = eff.Description, ID = eff.ID });


        foreach (var eff in db.AbilityEffects)
            AbilityEffects.Add(new Effect { Description = eff.Description, ID = eff.ID });


        foreach (var type in db.Types)
        {
            Types.Add(new ElementalType
            {
                ID = type.ID,
                Name = type.Name
            });

            foreach (var dr in type.DamageRelations)
                DamageRelations.Add(new DamageRelation
                {
                    AttackerID = type.ID, DefenderID = dr.DefendingType.ID,
                    ProzentualMultiplier = dr.ProcentualMultiplier
                });
        }


        foreach (var damageclass in db.DamageClasses)
            DamageClasses.Add(new DamageClass
            {
                ID = damageclass.ID, Name = damageclass.Name
            });


        foreach (var move in db.Moves)
            Moves.Add(new Move
            {
                ID = move.ID, Name = move.Name, EffectID = move.Effect.ID, EffectChance = move.EffectChance,
                Accuracy = move.Accuracy, Priority = move.Priority, Power = move.Power, PP = move.PP,
                DamageClassID = move.DamageClass.ID
            });

        Write();
    }
}