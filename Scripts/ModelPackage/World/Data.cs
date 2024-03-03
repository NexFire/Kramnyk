using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Godot;
using Newtonsoft.Json;

namespace ModelPackage.World
{
    public class SalesData
    {
        [JsonProperty]
        public int ProductId { get; private set; }
        [JsonProperty]
        public float AverageBuyPrice { get; private set; }
        [JsonProperty]
        public int OAPiecesBought { get; private set; }
        public int OAPiecesSold { get; private set; }
        [JsonProperty]
        public float AverageSellPrice { get; private set; }
        [JsonProperty]
        public int PiecesSold { get; private set; }
        public SalesData() { }
        public SalesData(NewSalesData newSaleData)
        {
            ProductId = newSaleData.ProductId;
            PiecesSold = newSaleData.PiecesSold;
            OAPiecesBought = newSaleData.OAPiecesBought;
            OAPiecesSold = newSaleData.OAPiecesSold;
            AverageBuyPrice = newSaleData.AverageBuyPrice;
            AverageSellPrice = newSaleData.AverageSellPrice;
            SetAverageBuyPrice(newSaleData.BuyPrices);
            SetAverageSellPrice(newSaleData.SellPrices);
        }
        public SalesData(SalesData saleData)
        {
            ProductId = saleData.ProductId;
            PiecesSold = 0;
            OAPiecesBought = saleData.OAPiecesBought;
            OAPiecesSold = saleData.OAPiecesSold;
            AverageBuyPrice = saleData.AverageBuyPrice;
            AverageSellPrice = saleData.AverageSellPrice;
        }
        public void SetAverageSellPrice(List<int> sellPrices)
        {

            if (sellPrices.Any())
            {
                var averageSum = OAPiecesSold * AverageSellPrice;
                OAPiecesSold += sellPrices.Count;
                AverageSellPrice = ((float)sellPrices.Sum() + averageSum) / OAPiecesSold;
            }
        }
        public void SetAverageBuyPrice(List<int> buyPrices)
        {
            if (buyPrices.Any())
            {
                var averageSum = OAPiecesBought * AverageBuyPrice;
                OAPiecesBought += buyPrices.Count;
                AverageBuyPrice = ((float)buyPrices.Sum() + averageSum) / OAPiecesBought;
            }
        }

        public void SoldPieces(int soldPieces)
        {
            if (soldPieces > 0)
            {
                PiecesSold += soldPieces;
            }

        }
        public void SetSoldPieces(int soldPieces)
        {
            PiecesSold = soldPieces;
        }
        public static List<SalesData> LoadSalesData(string salesDataFile)
        {
            try
            {
                string json = File.ReadAllText(salesDataFile);
                var availableSalesData = JsonConvert.DeserializeObject<List<SalesData>>(json);
                return availableSalesData ?? new List<SalesData>();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return new List<SalesData>();
            }
        }
    }
    public class NewSalesData : SalesData
    {
        public List<int> SellPrices = new();
        public List<int> BuyPrices = new();

        public NewSalesData(SalesData saleData) : base(saleData)
        {

        }
        public static List<SalesData> LoadNewSalesDataToOldSalesData(List<NewSalesData> newSalesDatas)
        {
            List<SalesData> salesData = new();
            foreach (var newSaleData in newSalesDatas)
            {
                salesData.Add(new SalesData(newSaleData));
            }
            return salesData;
        }
        public static List<NewSalesData> LoadNewSalesData(List<SalesData> salesDatas)
        {
            List<NewSalesData> newSalesDatas = new();
            foreach (var saleData in salesDatas)
            {
                newSalesDatas.Add(new NewSalesData(saleData));
            }
            return newSalesDatas;
        }
    }
}