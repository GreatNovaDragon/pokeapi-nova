// See https://aka.ms/new-console-template for more information

using SimplePokemonAPI.Models;
using SimplePokemonAPI.Serializers;

var csv = new CSV_Serializer();
csv.OverwriteWithDataFromDB(await new Knowledgebase().GetDatabaseFromPokeAPIWithoutEffects("en"));
csv.Write();