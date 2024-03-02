using Godot;
using System;
using ModelPackage.World;
public partial class MainMenuButtons : Control
{
    private int count = 0;
    World world;
    // public Node playScene;
    //public Node optionScene;
    // Equivalent to GDScript's _ready() function
    private int countDone = 0;
    public override void _Ready()
    {
        count = 0;
        //playScene = ResourceLoader.Load<PackedScene>("res://Scenes/label.tscn").Instantiate();
        //optionScene = ResourceLoader.Load<PackedScene>("res://Scenes/label.tscn").Instantiate();
        world = GetNode<World>("/root/world");
        if (world.GameString == null)
        {
            world.LoadStartAssets("Assets/config/filesConfig.json");
        }
    }

    // Equivalent to GDScript's _input(event) function
    public override void _Input(InputEvent @event)
    {
        if (@event is InputEventKey eventKey && eventKey.Pressed)
        {
            if ((eventKey.Keycode == Key.Tab || eventKey.Keycode == Key.Left ||
                 eventKey.Keycode == Key.Right || eventKey.Keycode == Key.Up ||
                 eventKey.Keycode == Key.Down) && count == 0)
            {
                count += 1;
                GetNode<Button>("PlayButton").GrabFocus();
            }
        }
    }

    // This method remains empty if no processing is needed.

    // Button signal handlers
    public void OnPlayButtonPressed()
    {
        world.StartNewGame();
        GetTree().ChangeSceneToPacked(ResourceLoader.Load<PackedScene>("res://Scenes/SelectItemsScreen.tscn"));
    }
    public void OnLoadOldGamePressed()
    {

        GetTree().ChangeSceneToPacked(ResourceLoader.Load<PackedScene>("res://Scenes/PickOldGame.tscn"));
    }
    public void OnOptionsButtonPressed()
    {

        GetTree().ChangeSceneToPacked(ResourceLoader.Load<PackedScene>("res://Scenes/OptionMenu.tscn"));
    }

    public void OnQuitButtonPressed()
    {
        // Assuming 'world' is accessible and has myArray and availableItems properties.
        // You need to have 'world' defined and accessible in this context.
        OS.Alert("Ahoj, jsi matlák", "Důležité oznámení");
        GD.Print(world.modelPath);
    }
}