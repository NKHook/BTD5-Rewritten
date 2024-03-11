extends Node2D

@export var color: Color = Color.BLACK
@export var speed: float = 1.0

# Called when the node enters the scene tree for the first time.
func _ready():
	pass # Replace with function body.


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	color.a -= delta * speed
	if color.a <= 0.0:
		color.a = 0.0
	queue_redraw()

func _draw():
	var viewport = get_viewport_rect()
	draw_rect(viewport, color)
	pass
