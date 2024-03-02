using System.Linq;
using Godot;
using ModelPackage.World;
public partial class SelectItems : Control
{
    World world;
    Label moneyLabel;
    Label spaceLabel;
    public override void _Ready()
    {
        world = GetNode<World>("/root/world");
        CycleStart();
        moneyLabel = GetNode<Label>("VBoxContainer/HBoxContainer/MoneyAmount/MoneyAmountValue");
        spaceLabel = GetNode<Label>("VBoxContainer/HBoxContainer/AvailableSpace/AvailableSpaceValue");
        var _vBoxCont = GetNode<VBoxContainer>("VBoxContainer/ScrollContainer/VBoxContainer");
        PackedScene itemWidget = ResourceLoader.Load<PackedScene>("res://widgets/ItemWidget/ItemWidgetV1.tscn");
        if (itemWidget != null)
        {
            foreach (var market in world.markets)
            {
                foreach (var buyableItem in market.buyableItems)
                {
                    Node widget = itemWidget.Instantiate();
                    if (widget is ItemWidget itemWidget1)
                    {
                        itemWidget1.LoadItemWidget(buyableItem, world);
                    }
                    _vBoxCont.AddChild(widget);
                }
            }
        }
        SetMoneyAndSpace();
        base._Ready();
    }
    private void CycleStart()
    {
        world.newSalesData = NewSalesData.LoadNewSalesData(world.oldSalesData);
        foreach (var market in world.markets)
        {
            market.GenerateItems(world.availableItems);
        }
        GD.Print(world.GameString);
        foreach (var merchant in world.merchants.Skip(1))
        {
            merchant.BuyItems(world.availableItems, world.markets, world.oldSalesData);
        }
        GD.Print("AAA");
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
    public void SetMoneyAndSpace()
    {
        moneyLabel.Text = world.merchants[0].Money.ToString();
        spaceLabel.Text = world.merchants[0].AvailableSpace.ToString();
    }
    public void ContinueCycle()
    {
        GetTree().ChangeSceneToPacked(ResourceLoader.Load<PackedScene>("res://Scenes/SummaryScreen.tscn"));
    }
}