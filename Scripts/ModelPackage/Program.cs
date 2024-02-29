using ModelPackage.World;
using System;

// See https://aka.ms/new-console-template for more information
/*
class Program
{
    static unsafe void Main()
    {
        World world =
            new World("Assets/config/filesConfig.json");
        //world.markets[4].GenerateItems(world.availableItems);
        //var test = world.GetOldGames();
        //world.LoadOldGame(test[0]);
        world.StartNewGame();
        for (int a = 0; a < 5; a++)
        {
            foreach (var market in world.markets)
            {
                market.GenerateItems(world.availableItems);
            }
            //world.merchants[0].BuyItemsUser(new List<List<int>> { new() { 0, 1 }, new() { 1, 1 } }, world.markets);
            foreach (var merchant in world.merchants.Skip(1))
            {
                merchant.BuyItems(world.availableItems, world.markets, world.oldSalesData);
            }
            world.GenerateWantedItems();
            world.BuyWantedItems();
            foreach (var merchant in world.merchants)
            {
                merchant.RemoveNotKeepable();
            }
            Market.ClearItems(world.markets);
        }
        world.SaveGame();
        Console.WriteLine(world.availableItems[0].Name["cz"]);
    }
}
*/

