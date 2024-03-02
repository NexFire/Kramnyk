using Godot;

public partial class PauseMenuScript : Control
{
    World world;
    public override void _Ready()
    {
        world = GetNode<World>("/root/world");
        base._Ready();
    }
    public void OnQuitButton()
    {
        world.SaveGame();
        GetTree().ChangeSceneToPacked(ResourceLoader.Load<PackedScene>("res://Scenes/MainMenu.tscn"));
    }
}