using System;
using System.IO;
using Newtonsoft.Json;
//using System.Collections.Generic;
using System.Linq;
using Godot;
using ModelPackage.World;
using System.Collections.Generic;


public partial class World : Node
{
    public World()
    {
        modelPath = ProjectSettings.GlobalizePath("res://Scripts/ModelPackage/");
    }
    public string GetAssetPath(string assetName)
    {
        if (GameString != null)
        {
            return Path.Combine(GameString, (string)gameFilesConfig[assetName]);
        }
        else
        {
            return Path.Combine(modelPath, (string)filesConfig["configAssetsPath"], (string)filesConfig[assetName]);
        }
    }
    public void LoadStartAssets(string configFileThing)
    {
        filesConfig = LoadFilesConfig(Path.Combine(modelPath, configFileThing), RequiredArgs);
        availableItems = AvailableItem.LoadItems(GetAssetPath("items"));
        markets = Market.LoadMarkets(GetAssetPath("markets"));
        storages = Storage.LoadStorages(GetAssetPath("storages"));
    }
    public Dictionary<string, object> LoadFilesConfig(string filesConfigPath, List<string> RequiredArgs)
    {
        try
        {
            string json = File.ReadAllText(filesConfigPath);
            var filesConfig = JsonConvert.DeserializeObject<Dictionary<string, object>>(json) ?? throw new Exception("The filesConfig is empty you need to fill it up");
            CheckAtributes(filesConfig, RequiredArgs);
            return filesConfig ?? new Dictionary<string, object>();
        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            return new Dictionary<string, object>();
        }
    }
    private void CheckAtributes(Dictionary<string, object> dict, List<string> keys)
    {
        foreach (var key in keys)
        {
            if (!dict.ContainsKey(key))
            {
                throw new System.Exception(key + " was not found in the keys of the dictionary");
            }
        }
    }


    public void LoadGameAssets()
    {
        GD.Print("Yes we are loading this");
        gameFilesConfig = LoadFilesConfig(Path.Combine(GameString, "filesConfig.json"), GameFileRequiredArgs);
        merchants = Merchant.LoadMerchants(GetAssetPath("bots"));
        oldSalesData = SalesData.LoadSalesData(GetAssetPath("salesData"));
        GD.Print(oldSalesData);
        newSalesData = NewSalesData.LoadNewSalesData(oldSalesData);
    }
    public void LoadOldGame(string oldGameString)
    {
        GameString = Path.Combine(modelPath, "Assets/runData", oldGameString);
        LoadGameAssets();
    }
    public List<List<string>> GetOldGames()
    {
        DirectoryInfo dirInfo = new DirectoryInfo(Path.Combine(modelPath, "Assets/runData"));

        // Get all subdirectories
        DirectoryInfo[] directories = dirInfo.GetDirectories();

        // Sort the directories by last write time in descending order
        var sortedDirectoriesWithEditTime = directories.Select(d => new
        {
            DirectoryName = d.Name,
            LastEditTime = d.GetFiles("*", SearchOption.AllDirectories) // Include all subdirectories
                         .OrderByDescending(f => f.LastWriteTime)
                         .FirstOrDefault()?.LastWriteTime // Gets the last write time of the most recently modified file
        })
    .Where(d => d.LastEditTime != null) // Filter out directories without files
    .OrderByDescending(d => d.LastEditTime) // Sort by the last edit time
    .Select(d => new List<string> { d.DirectoryName, d.LastEditTime?.ToString("G") }) // Select as List of strings
    .ToList();
        //List<string> directories = new(Directory.GetDirectories(Path.Combine(modelPath, "Assets/runData")));
        return sortedDirectoriesWithEditTime;
    }
    public void SaveGame()
    {
        try
        {
            if (GameString == null)
            {
                throw new Exception("You are not in game therefore you cant save the game");
            }
            string merchantsJson = JsonConvert.SerializeObject(merchants, Formatting.Indented);
            string filePath = Path.Combine(GameString, "merchants.json");
            File.WriteAllText(filePath, merchantsJson);
            Console.WriteLine("Soubor zapsán");
        }
        catch (Exception e)
        {
            Console.WriteLine(e.Message);
        }

    }
    //This is the starting new game
    public void StartNewGame()
    {
        try
        {
            string timestamp = DateTime.Now.ToString("yyyy_MM_dd_HH_mm_ss");
            string folderName = $"GameFolder_{timestamp}";
            string directoryPath = Path.Combine(modelPath, "Assets/runData", folderName);
            if (!Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
                GD.Print($"Folder '{directoryPath}' created successfully.");
            }
            else
            {
                GD.Print($"Folder '{directoryPath}' already exists.");
            }
            string[] files = Directory.GetFiles(Path.Combine(modelPath, (string)filesConfig["configAssetsPath"], (string)filesConfig["copyFilesPath"]));
            foreach (string filePath in files)
            {
                // Get the file name from the path
                string fileName = Path.GetFileName(filePath);
                // Create the destination file path
                string destFile = Path.Combine(directoryPath, fileName);
                // Copy the file
                File.Copy(filePath, destFile, true); // Set to 'true' to overwrite existing files
            }
            GameString = directoryPath;
            LoadGameAssets();
            merchants = Merchant.GenerateMoney(merchants);
        }
        catch (System.Exception e)
        {
            GD.Print($"This is the error: {e.Message}");
        }

    }
    public void GenerateWantedItems()
    {
        System.Random random = new();
        wantedItems = new();
        var oneDayMargin = 2 * random.NextDouble() + 1;
        foreach (var item in availableItems)
        {
            int amount = (int)(item.Importance * random.Next(1, 51));
            int price = (int)(item.MinPrice * oneDayMargin);
            if (amount != 0 && price != 0)
            {
                wantedItems.Add(new Item(item, price, amount));
            }
        }
    }
    public void BuyWantedItems()
    {
        foreach (var wantedItem in wantedItems)
        {
            var sortedMerchants = merchants.OrderBy(merchant => merchant.OwnedItems.FirstOrDefault(item => item.Id == wantedItem.Id)?.Price ?? int.MaxValue).ToList();
            var salesItemData = newSalesData.FirstOrDefault(item => item.ProductId == wantedItem.Id, null);
            if (salesItemData == null)
            {
                throw new Exception("This item has no data");
            }
            foreach (var sortedMerchant in sortedMerchants)
            {
                sortedMerchant.SellItem(wantedItem, salesItemData);
            }
        }

    }

    public string modelPath;
    private List<string> RequiredArgs = new() { "configAssetsPath", "items", "markets" };
    private List<string> GameFileRequiredArgs = new() { "bots", "salesData" };
    private Dictionary<string, object> filesConfig;
    public Godot.Collections.Dictionary<string, int> test = new() { { "test", 2 }, { "pest", 3 } };
    private Dictionary<string, object> gameFilesConfig;
    public string GameString { get; set; }
    public List<Item> wantedItems;
    public List<Storage> storages;
    public List<SalesData> oldSalesData;
    public List<NewSalesData> newSalesData;
    public List<Merchant> merchants;
    public List<AvailableItem> availableItems;
    public List<Market> markets;
}