using System.Collections.Generic;
using Godot;

public partial class SelectOldGame : Control
{
    ItemList itemList;
    List<List<string>> filesAndTimes;
    World world;
    public override void _Ready()
    {
        itemList = GetNode<ItemList>("VBoxContainer/GameFiles");
        world = GetNode<World>("/root/world");
        filesAndTimes = world.GetOldGames();
        foreach (var fileAndTime in filesAndTimes)
        {
            itemList.AddItem(fileAndTime[0]);
        }
    }
    public void SelectOldGameFile()
    {
        var index = itemList.GetSelectedItems();
        world.LoadOldGame(filesAndTimes[index[0]][0]);
        GetTree().ChangeSceneToPacked(ResourceLoader.Load<PackedScene>("res://Scenes/SelectItemsScreen.tscn"));
    }
}