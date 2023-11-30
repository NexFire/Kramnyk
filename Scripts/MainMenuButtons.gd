extends VBoxContainer

@export var sceneToLoad:PackedScene

var label2=Label.new()
# Called when the node enters the scene tree for the first time.
func _ready():
	print("this is test on init ",world.testGetter())


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
