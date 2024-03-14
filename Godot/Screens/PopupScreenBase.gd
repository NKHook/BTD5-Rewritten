extends Node2D

class_name PopupScreenBase

func _ready():
	var close_button = $popup_layer/close_button as SpriteButton
	close_button.pressed.connect(_close_button_pressed)
	
func _close_button_pressed():
	queue_free()
