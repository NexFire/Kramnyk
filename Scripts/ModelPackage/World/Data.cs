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
        public float AverageSellPrice { get; private set; }
        [JsonProperty]
        public int PiecesSold { get; private set; }
        public SalesData() { }
        public SalesData(int Id)
        {
            ProductId = Id;
            PiecesSold = 0;
        }
        public void SetAverageSellPrice(List<int> sellPrices)
        {
            if (sellPrices.Any())
            {
                GD.Print("S1");
                if (sellPrices.Count > PiecesSold)
                {
                    GD.Print(PiecesSold);
                    AverageSellPrice = (float)sellPrices.Take(PiecesSold).Average();
                }
                GD.Print("S2");
                if (sellPrices.Count < PiecesSold)
                {
                    PiecesSold = sellPrices.Count;
                }
                GD.Print("S3");
                AverageSellPrice = (float)sellPrices.Average();
                GD.Print("S4");
            }

        }
        public void SetAverageBuyPrice(List<int> buyPrices)
        {
            if (buyPrices.Any())
            {
                if (buyPrices.Count > PiecesSold)
                {
                    AverageSellPrice = (float)buyPrices.Take(PiecesSold).Average();
                }

                if (buyPrices.Count < PiecesSold)
                {
                    PiecesSold = buyPrices.Count;
                }
                AverageSellPrice = (float)buyPrices.Average();
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

        public NewSalesData(SalesData saleData) : base(saleData.ProductId)
        {

        }
        public static List<SalesData> LoadNewSalesDataToOldSalesData(List<NewSalesData> newSalesDatas)
        {
            List<SalesData> salesData = new();
            foreach (var newSaleData in newSalesDatas)
            {
                GD.Print("T1");
                salesData.Add(new SalesData(newSaleData.ProductId));
                GD.Print("T2");
                var item = salesData.Last();
                GD.Print("T3");
                item.SetSoldPieces(newSaleData.PiecesSold);
                GD.Print("T4");
                item.SetAverageBuyPrice(newSaleData.BuyPrices);
                GD.Print("T5");
                item.SetAverageSellPrice(newSaleData.SellPrices);
                GD.Print("T6");
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