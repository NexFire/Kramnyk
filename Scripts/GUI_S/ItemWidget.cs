using System;
using System.Linq;
using Godot;
using ModelPackage.World;
public partial class ItemWidget : Control
{
    World world;
    int itemCount = 0;
    Item localItem;
    Label amountLabel;
    public override void _Ready()
    {
        world = GetNode<World>("/root/world");
        base._Ready();
    }
    public void LoadItemWidget(Item item, World world)
    {
        localItem = item;
        this.world = world;
        var saleData = world.oldSalesData.FirstOrDefault(saleDataItem => saleDataItem.ProductId == item.Id, null);
        if (saleData == null)
        {
            return;
        }
        var itemName = GetNode<Label>("MarginContainer/MainContainer/ItemName");
        var priceLabel = GetNode<Label>("MarginContainer/MainContainer/SpliterContainer/InfoSide/Price/PriceValue");
        amountLabel = GetNode<Label>("MarginContainer/MainContainer/SpliterContainer/InfoSide/Amount/AmountValue");
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
    public void OnBuyButtonPress()
    {
        var priceInput = GetNode<SpinBox>("MarginContainer/MainContainer/SpliterContainer/SettingSide/Price/HBoxContainer/SpinBox");
        var amountInput = GetNode<SpinBox>("MarginContainer/MainContainer/SpliterContainer/SettingSide/Amount/HBoxContainer/SpinBox");
        var user = world.merchants.FirstOrDefault(merchant => merchant.Id == 0, null);
        if (user == null)
        {
            return;
        }
        user.BuyItemsUser(new() { new() { localItem.Id, (int)amountInput.Value } }, world.markets);
        user.Margin = (float)priceInput.Value;
        amountLabel.Text = localItem.Amount.ToString();
        var parentScript = GetNodeOrNull<SelectItems>("../../../../../GameScreen");
        if (parentScript == null)
        {
            return;
        }
        parentScript.SetMoneyAndSpace();
    }
}