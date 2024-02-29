using System;
using System.Linq;
using Godot;
using ModelPackage.World;
public partial class ItemWidget : Control
{
    World world;
    int itemCount = 0;
    Item localItem;
    public override void _Ready()
    {
        world = GetNode<World>("/root/world");
        base._Ready();
    }
    public void LoadItemWidget(Item item, World world)
    {
        localItem = item;
        GD.Print("This is in the item");
        GD.Print(item);
        GD.Print(world);
        GD.Print("This is after the problem");
        var saleData = world.oldSalesData.FirstOrDefault(saleDataItem => saleDataItem.ProductId == item.Id, null);
        if (saleData == null)
        {
            return;
        }
        var itemName = GetNode<Label>("MarginContainer/MainContainer/ItemName");
        var priceLabel = GetNode<Label>("MarginContainer/MainContainer/SpliterContainer/InfoSide/Price/PriceValue");
        var amountLabel = GetNode<Label>("MarginContainer/MainContainer/SpliterContainer/InfoSide/Amount/AmountValue");
        var avgSellPriceLabel = GetNode<Label>("MarginContainer/MainContainer/SpliterContainer/InfoSide/AverageSellPrice/AverageSellPriceValue");
        var avgBuyPriceLabel = GetNode<Label>("MarginContainer/MainContainer/SpliterContainer/InfoSide/AverageBuyPrice/AverageBuyPriceValue");
        var soldPiecesLabel = GetNode<Label>("MarginContainer/MainContainer/SpliterContainer/InfoSide/PiecesSold/PiecesSoldValue");
        var sizeLabel = GetNode<Label>("MarginContainer/MainContainer/SpliterContainer/InfoSide/SpaceComplexity/SpaceComplexityValue");
        itemName.Text = item.Name["cz"];
        priceLabel.Text = item.Price.ToString();
        amountLabel.Text = item.Amount.ToString();
        avgSellPriceLabel.Text = saleData.AverageSellPrice.ToString();
        avgBuyPriceLabel.Text = saleData.AverageBuyPrice.ToString();
        soldPiecesLabel.Text = saleData.PiecesSold.ToString();
        sizeLabel.Text = item.SpaceComplexity.ToString();
    }
}