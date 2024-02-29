using System.Linq;
using Godot;
public partial class SelectItems : Control
{
    World world;
    public override void _Ready()
    {
        world = GetNode<World>("/root/world");
        GD.Print("Yes we here");
        CycleStart();
        GD.Print("Yep this is a thing lol");
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
        base._Ready();
    }
    private void CycleStart()
    {
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
}