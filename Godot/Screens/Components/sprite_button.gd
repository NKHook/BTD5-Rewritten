extends Button

class_name SpriteButton

@export var sprite_name: String = ""
@export var texture_name: String = ""

# Called when the node enters the scene tree for the first time.
func _ready():
	var cell = TextureLoader.FindCell(sprite_name, texture_name)
	var sprite_obj = Sprite2D.new()
	
	var factor: Vector2 = size / Vector2(cell.W, cell.H)
	sprite_obj.scale = factor
	
	sprite_obj.centered = false
	sprite_obj.texture = cell.GetTexture()
	sprite_obj.region_enabled = true;
	sprite_obj.region_rect = Rect2(cell.X, cell.Y, cell.W, cell.H)
	
	add_child(sprite_obj)
