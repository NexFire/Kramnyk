using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Newtonsoft.Json;

namespace ModelPackage.World
{
    public class Market
    {
        public int Id { get; set; }
        public Dictionary<string, string> Name { get; set; }
        public Dictionary<int, float> Goods { get; set; }
        public Dictionary<string, Dictionary<string, string>> Location { get; set; }

        public List<Item> buyableItems = new();
        public bool GenerateItems(List<AvailableItem> availableItems)
        {
            int minPrice;
            int amount;
            Random random = new();
            AvailableItem? item;

            foreach (var key in Goods.Keys)
            {
                item = availableItems.FirstOrDefault(aI => aI.Id == key)!;
                if (item == null)
                {
                    throw new Exception("There is no such item");
                }
                minPrice = item.MinPrice;
                amount = (int)(Goods[key] * random.Next(1, 101));
                if (amount != 0)
                {
                    buyableItems.Add(new Item(item, minPrice + (int)(random.NextDouble() * minPrice), amount));
                }
            }
            return true;
        }
        public static void ClearItems(List<Market> markets)
        {
            foreach (var market in markets)
            {
                market.buyableItems.Clear();
            }
        }
        public static List<Market> LoadMarkets(string marketsFile)
        {
            try
            {
                Console.WriteLine(marketsFile);
                string json = File.ReadAllText(marketsFile);
                var availableMarkets = JsonConvert.DeserializeObject<List<Market>>(json);
                return availableMarkets ?? new List<Market>();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return new List<Market>();
            }
        }

        //this is for getting the items from here if you excede the amount of items
        public void BuyItem(int itemId, int amount, Merchant merchant)
        {
            var item = buyableItems.First(inMarketItem => inMarketItem.Id == itemId);
            if (item.Amount == 0)
            {
                return;
            }
            if (amount > item.Amount)
            {
                amount = item.Amount;
            }
            var copyItem = DeepCopy(item);
            copyItem.Amount = amount;
            if (merchant.GetItem(copyItem))
            {
                item.Amount -= amount;
            }

        }
        public static T DeepCopy<T>(T obj)
        {
            string json = JsonConvert.SerializeObject(obj);
            var result = JsonConvert.DeserializeObject<T>(json) ?? throw new InvalidOperationException("Deserialization is null");
            return result;
        }
    }
}