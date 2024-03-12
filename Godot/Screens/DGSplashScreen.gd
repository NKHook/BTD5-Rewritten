extends Node2D

@export var screen_manager: Node2D
const main_menu = preload("res://Godot/Screens/MainMenuScreen.tscn")

var time: float = 0.0

# Called when the node enters the scene tree for the first time.
func _ready():
	time = 0.0


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	time += delta
	if time > 1.2 and screen_manager != null:
		screen_manager.add_child(main_menu.instantiate())
		screen_manager.remove_child(self)
