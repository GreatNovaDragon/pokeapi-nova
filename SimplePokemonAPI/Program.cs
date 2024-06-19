// See https://aka.ms/new-console-template for more information

using SimplePokemonAPI.FileModels;
using SimplePokemonAPI.Models;

Console.WriteLine("Hello, World!");
var csvdb = new CSV();
csvdb.PrepareEmptyDatabase();
FileModel.OverwriteWithDataFromDB(new Database());