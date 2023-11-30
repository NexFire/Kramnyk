extends VBoxContainer

var count
# Called when the node enters the scene tree for the first time.
func _ready():
	count=0

func _input(event):
	if event is InputEventKey and event.pressed:
		if ((event.keycode == KEY_TAB or event.keycode == KEY_LEFT or event.keycode == KEY_RIGHT or event.keycode == KEY_UP or event.keycode == KEY_DOWN)and count==0):
			count+=1
			get_node("PlayButton").grab_focus()


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	pass


func _on_PlayButton_pressed():
	world.testSetter(5)
	get_tree().change_scene_to_file("res://Scenes/label.tscn")
func _on_OptionsButton_pressed():
	get_tree().change_scene_to_file("res://Scenes/StoryScene.tscn")
func _on_QuitButton_pressed():
	pass
