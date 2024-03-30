extends Camera2D

var zoom_factor: Vector2 = Vector2(1, 1)
@export var tracker: Node = null;

# Called when the node enters the scene tree for the first time.
func _ready():
	pass # Replace with function body.


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	if tracker != null:
		position = tracker.position
	if Input.is_key_pressed(KEY_UP):
		self.zoom += zoom_factor * delta
	if Input.is_key_pressed(KEY_DOWN):
		self.zoom -= zoom_factor * delta
	
	
