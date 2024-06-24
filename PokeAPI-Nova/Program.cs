// See https://aka.ms/new-console-template for more information

using SimplePokemonAPI.Models;
using SimplePokemonAPI.Serializers;

var csv = new CSV_Serializer();
var knowledgebase = await new Database().GetDatabaseFromPokeAPIWithoutEffects("en");

foreach (var item in knowledgebase.Pokemon) Console.WriteLine($"{item.Name} ({item.FormName})");

csv.OverwriteWithDataFromDB(knowledgebase);
csv.Write();