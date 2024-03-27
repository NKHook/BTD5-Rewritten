extends Node2D

@export var radius: float = 8.0;
@export var color: Color = Color.WHITE;

func _draw():
	draw_circle(position, radius, color)
