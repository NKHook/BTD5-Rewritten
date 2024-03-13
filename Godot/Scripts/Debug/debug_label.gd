extends Label

class_name DebugLabel

# Called when the node enters the scene tree for the first time.
func _ready():
	position.y = get_index() * 25


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	pass
