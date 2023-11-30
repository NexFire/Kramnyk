extends Node2D

var text = "Your Text"
var radius = 100  # Adjust as needed

func _ready():
	var angle_start = -PI / 2  # Start at the bottom of the semicircle
	var char_spacing = PI / (text.length - 1)  # Spacing for half-circle

	for i in range(text.length):
		var char = text[i]
		var label = Label.new()
		label.text = str(char)
		add_child(label)

		var angle = angle_start + i * char_spacing
		label.rect_position = Vector2(
			radius * cos(angle) + radius,  # X position
			radius * sin(angle) + radius   # Y position
		)
