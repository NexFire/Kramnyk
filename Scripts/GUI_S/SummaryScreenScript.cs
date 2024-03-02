using Godot;
using ModelPackage.World;
public partial class SummaryScreenScript : Control
{
    World world;
    public override void _Ready()
    {
        world = GetNode<World>("/root/world");
        world.GenerateWantedItems();
        world.BuyWantedItems();
        GD.Print("This is a thing");
        world.oldSalesData = NewSalesData.LoadNewSalesDataToOldSalesData(world.newSalesData);
        GD.Print("this isůfjůlasdlkjs");
        Market.ClearItems(world.markets);
        var tree = GetNode<Tree>("VBoxContainer/Tree");

        TreeItem root = tree.CreateItem();
        TreeItem childMerchant;
        TreeItem subChild;
        foreach (var merchant in world.merchants)
        {
            merchant.RemoveNotKeepable();
            childMerchant = tree.CreateItem(root);
            childMerchant.SetText(0, merchant.Name["cz"]);
            subChild = tree.CreateItem(childMerchant);
            subChild.SetText(0, "Peníze");
            subChild.SetText(1, merchant.Money.ToString());
            subChild = tree.CreateItem(childMerchant);
            subChild.SetText(0, "Sklad");
            subChild.SetText(1, merchant.AvailableSpace.ToString());
        }
        base._Ready();
    }
    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventKey eventKey && eventKey.Pressed)
        {
            if (eventKey.Keycode == Key.Escape)
            {
                var pauseMenu = GetNode<Control>("PauseMenu");
                if (pauseMenu.Visible)
                {
                    pauseMenu.Hide();
                }
                else
                {
                    pauseMenu.Show();
                }
            }
        }
        base._Input(@event);
    }
    public void OnContinuePressed()
    {
        GetTree().ChangeSceneToPacked(ResourceLoader.Load<PackedScene>("res://Scenes/SelectItemsScreen.tscn"));
    }
}