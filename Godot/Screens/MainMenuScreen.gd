extends Node2D

@export var intro_duration: float = 7.0

var time_passed: float = 0.0

# Called when the node enters the scene tree for the first time.
func _ready():
	pass # Replace with function body.


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	time_passed += delta;
	if time_passed > intro_duration:
		var camera_follow = $camera_path/camera_follow as PathFollow2D
		camera_follow.progress_ratio += delta
		
	if Input.is_mouse_button_pressed(MOUSE_BUTTON_LEFT):
		time_passed = intro_duration
