using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
            if (sellPrices.Count > PiecesSold)
            {
                AverageSellPrice = (float)sellPrices.Take(PiecesSold).Average();
            }
            if (sellPrices.Count < PiecesSold)
            {
                PiecesSold = sellPrices.Count;
            }
            AverageSellPrice = (float)sellPrices.Average();
        }
        public void SetAverageBuyPrice(List<int> buyPrices)
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

        public void SoldPieces(int soldPieces)
        {
            if (soldPieces <= 0)
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
        public List<SalesData> LoadNewSalesDataToOldSalesData(List<NewSalesData> newSalesDatas)
        {
            List<SalesData> salesData = new();
            foreach (var newSaleData in newSalesDatas)
            {
                salesData.Add(new SalesData(newSaleData.ProductId));
                var item = salesData.Last();
                item.SetSoldPieces(newSaleData.PiecesSold);
                item.SetAverageBuyPrice(newSaleData.BuyPrices);
                item.SetAverageSellPrice(newSaleData.SellPrices);
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