extends Button

class_name SpriteButton

@export var sprite_name: String = ""
@export var texture_name: String = ""

# Called when the node enters the scene tree for the first time.
func _ready():
	var sprite = Sprite.new()
	sprite.centered = false
	sprite.sprite_name = sprite_name
	sprite.texture_name = texture_name
	
	var cell = TextureLoader.FindCell(sprite_name, texture_name)
	var factor: Vector2 = size / Vector2(cell.W, cell.H)
	sprite.scale = factor
	
	add_child(sprite)
	move_child(sprite, 0)
