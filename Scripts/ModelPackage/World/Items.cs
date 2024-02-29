using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;
using Godot;

namespace ModelPackage.World
{
    public class AvailableItem
    {
        public void LoadItem(AvailableItem item)
        {
            Id = item.Id;
            Name = item.Name;
            Icon = item.Icon;
            Unit = item.Unit;
            MinPrice = item.MinPrice;
            SpaceComplexity = item.SpaceComplexity;
            Keepable = item.Keepable;
        }
        public int Id { get; set; }
        public Dictionary<string, string> Name { get; set; }
        public string Icon { get; set; }
        public string Unit { get; set; }
        public int MinPrice { get; set; }
        public int SpaceComplexity { get; set; }
        public bool Keepable { get; set; }
        public float Importance { get; set; }
        public static List<AvailableItem> LoadItems(string itemsFile)
        {
            try
            {
                string json = File.ReadAllText(itemsFile);
                var availableItems = JsonConvert.DeserializeObject<List<AvailableItem>>(json);
                return availableItems ?? new List<AvailableItem>();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return new List<AvailableItem>();
            }
        }
    }

    public class Item : AvailableItem
    {
        public Item() { }
        public Item(AvailableItem item, int price, int amount)
        {
            LoadItem(item);
            Price = price;
            Amount = amount;
        }
        public int Price { get; set; }
        public int Amount { get; set; }

    }
}