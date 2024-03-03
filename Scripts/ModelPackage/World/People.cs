using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Runtime.CompilerServices;
using Godot;
using Newtonsoft.Json;

namespace ModelPackage.World
{
    public class Person
    {
        public int Id { get; set; }
        [JsonProperty]
        public int AvailableSpace { get; private set; }
        public string icon = "";
        [JsonProperty]
        public int Money { get; private set; }
        public int GetMoney()
        {
            return Money;
        }
        public void SetMoney(int money)
        {
            Money = money;
        }
        public bool DeltaMoneyPossible(int delta)
        {
            if (Money + delta < 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        public bool DeltaSpacePossible(int delta)
        {
            if (AvailableSpace + delta < 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }
        public void DeltaSpace(int delta)
        {
            AvailableSpace += delta;
        }
        public void DeltaMoney(int delta)
        {
            Money += delta;
        }
    }
    public class Merchant : Person
    {
        public Dictionary<string, string> Name;
        public float Margin { get; set; }
        public float MarginLimit { get; set; }
        [JsonProperty]
        public int StorageIndex { get; private set; }
        private List<Item> _ownedItems = new();
        public List<int> PriorityItems = new();

        [JsonProperty] // This attribute helps Newtonsoft.Json to recognize the property to serialize/deserialize
        public List<Item> OwnedItems
        {
            get => _ownedItems;
            set => _ownedItems = value ?? new List<Item>(); // Ensure it's never set to null
        }
        public static List<Merchant> LoadMerchants(string merchantsFile)
        {
            try
            {
                string json = File.ReadAllText(merchantsFile);
                var availableMerchants = JsonConvert.DeserializeObject<List<Merchant>>(json);
                return availableMerchants ?? new List<Merchant>();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return new List<Merchant>();
            }
        }
        public void CalculatePriority(List<AvailableItem> items, List<Market> markets, List<SalesData> oldSalesDatas)
        {
            List<List<float>> priorityItems = new();
            foreach (var oldSaleData in oldSalesDatas)
            {
                int piecesSold;
                if (oldSaleData.PiecesSold == 0)
                {
                    piecesSold = 1;
                }
                else
                {
                    piecesSold = oldSaleData.PiecesSold;
                }
                priorityItems.Add(new List<float> { oldSaleData.ProductId, (oldSaleData.AverageSellPrice - oldSaleData.AverageBuyPrice) * piecesSold });
            }
            priorityItems.Sort((list1, list2) => list2[1].CompareTo(list1[1]));
            priorityItems.RemoveAll(item =>
            {
                var foundItem = markets.First(market => market.Goods.ContainsKey((int)item[0])).buyableItems.FirstOrDefault(inMarketItem => inMarketItem.Id == (int)item[0], null);
                if (foundItem == null)
                {
                    return true;
                }
                var itemPrice = foundItem.Price;
                var itemData = oldSalesDatas.First(saleItem => saleItem.ProductId == (int)item[0]);
                var matchingItem = items.FirstOrDefault(item2 => item2.Id == item[0]);
                return matchingItem != null && (matchingItem.MinPrice > Money || matchingItem.SpaceComplexity > AvailableSpace || itemPrice * Margin > itemData.AverageSellPrice * MarginLimit);
            });
            foreach (List<float> list in priorityItems)
            {
                PriorityItems.Add((int)list[0]);
            }
        }
        public bool GetItem(Item item)
        {
            GD.Print("Player Id: " + Id + " Item id: " + item.Id);
            if (DeltaMoneyPossible(-1 * item.Price * item.Amount) && DeltaSpacePossible(-1 * item.SpaceComplexity * item.Amount))
            {
                DeltaMoney(-1 * item.Price * item.Amount);
                DeltaSpace(-1 * item.SpaceComplexity * item.Amount);
                if (Id == 0)
                {
                    item.Price = (int)Margin;
                }
                else
                {
                    item.Price = (int)(item.Price * Margin);
                }
                var ownedItem = OwnedItems.FirstOrDefault(ownedItemFinding => ownedItemFinding.Id == item.Id, null);
                if (ownedItem == null)
                {
                    GD.Print("Yes we are adding new item");
                    OwnedItems.Add(item);
                }
                else
                {
                    GD.Print("We are adding already existing");
                    if (item.Price > ownedItem.Price)
                    {
                        ownedItem.Price = item.Price;
                    }
                    ownedItem.Amount += item.Amount;
                }
                return true;
            }
            if (Id == 0)
            {
                OS.Alert("Nemáte dost místa nebo dost peněz na zakoupení položky");
            }
            return false;
        }
        public bool SellItem(Item item, NewSalesData newSaleData)
        {
            Console.WriteLine("This is test");
            var itemToSell = OwnedItems.FirstOrDefault(itemSearch => itemSearch.Id == item.Id, null);
            if (itemToSell == null)
            {
                return false;
            }
            if (itemToSell.Price > item.Price)
            {
                return false;
            }
            if (itemToSell.Amount <= item.Amount)
            {
                if (DeltaMoneyPossible(itemToSell.Price * itemToSell.Amount) && DeltaSpacePossible(itemToSell.Amount * itemToSell.SpaceComplexity))
                {
                    DeltaMoney(itemToSell.Price * itemToSell.Amount);
                    DeltaSpace(itemToSell.Amount * itemToSell.SpaceComplexity);
                    item.Amount -= itemToSell.Amount;
                    for (int a = 0; a < itemToSell.Amount; a++)
                    {
                        newSaleData.SellPrices.Add(itemToSell.Price);
                    }
                    newSaleData.SoldPieces(itemToSell.Amount);
                    OwnedItems.RemoveAll(itemSearch => itemSearch.Id == item.Id);
                }
            }
            else
            {
                if (DeltaMoneyPossible(itemToSell.Price * item.Amount) && DeltaSpacePossible(item.Amount * itemToSell.SpaceComplexity))
                {
                    DeltaMoney(itemToSell.Price * item.Amount);
                    DeltaSpace(item.Amount * itemToSell.SpaceComplexity);
                    itemToSell.Amount -= item.Amount;
                    newSaleData.SoldPieces(item.Amount);
                    for (int a = 0; a < item.Amount; a++)
                    {
                        newSaleData.SellPrices.Add(itemToSell.Price);
                    }
                    item.Amount = 0;
                }
            }
            return true;
        }
        public void BuyItems(List<AvailableItem> items, List<Market> markets, List<SalesData> oldSalesData, List<NewSalesData> newSalesDatas)
        {
            CalculatePriority(items, markets, oldSalesData);
            List<List<int>> counts = new();
            float tooHighCoeficient = 0.1f;
            foreach (var itemId in PriorityItems)
            {
                var market = markets.FirstOrDefault(market => market.buyableItems.Any(buyItem => buyItem.Id == itemId), null);
                if (market == null)
                {
                    return;
                }
                var item = market?.buyableItems.FirstOrDefault(inMarketItem => inMarketItem.Id == itemId, null);
                if (item == null)
                {
                    return;
                }
                var itemData = oldSalesData.FirstOrDefault(saleItem => saleItem.ProductId == itemId, null);
                if (itemData == null)
                {
                    return;
                }
                var newSaleData = newSalesDatas.FirstOrDefault(data => data.ProductId == itemId, null);
                if (newSaleData == null)
                {
                    return;
                }
                int itemCount = GetCount(itemId, item, itemData);
                while (itemCount * item.SpaceComplexity > AvailableSpace || itemCount * item.Price > Money)
                {
                    itemCount = (int)(itemCount * tooHighCoeficient);
                }
                if (itemCount != 0)
                {
                    market?.BuyItem(itemId, itemCount, this, newSaleData);
                }
            }
        }
        public void RemoveNotKeepable()
        {
            OwnedItems.RemoveAll(item =>
            {
                if (item.Keepable == false)
                {
                    DeltaSpace(item.Amount * item.SpaceComplexity);
                    return true;
                }
                return false;
            });
        }
        public void BuyItemsUser(List<List<int>> itemsToBuy, List<Market> markets, List<NewSalesData> newSalesDatas)
        {
            foreach (var item in itemsToBuy)
            {
                var market = markets.FirstOrDefault(market => market.Goods.ContainsKey(item[0]), null);
                if (market == null)
                {
                    return;
                }
                var newSaleData = newSalesDatas.FirstOrDefault(data => data.ProductId == item[0], null);
                if (newSaleData == null)
                {
                    return;
                }
                market?.BuyItem(item[0], item[1], this, newSaleData);
            }
            Console.WriteLine("Test");
        }
        public int GetCount(int productId, Item item, SalesData salesData)
        {
            int countHave = OwnedItems.FirstOrDefault(item => item.Id == productId)?.Amount ?? 1;
            int a = 60;
            float b = 0.4f;
            int piecesSold = salesData.PiecesSold;
            if (piecesSold == 0)
            {
                b = 35f;
            }
            else
            {
                b = 4f;
            }
            piecesSold = salesData.PiecesSold == 0 ? 1 : salesData.PiecesSold;
            int count = (int)(b * item.SpaceComplexity * countHave * (salesData.AverageSellPrice - salesData.AverageBuyPrice) * piecesSold * Money / (2 * item.Price * AvailableSpace));
            if (count < 0)
            {
                count = 0;
            }
            GD.Print("Player id: " + Id + " product Id: " + productId + " count: " + count);
            return count;
        }
        public static List<Merchant> GenerateMoney(List<Merchant> merchants)
        {
            Random random = new();
            foreach (var merchant in merchants)
            {
                merchant.SetMoney(random.Next(40, 81));
            }
            return merchants;
        }
    }
}