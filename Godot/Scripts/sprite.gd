extends Sprite2D

class_name Sprite

@export var sprite_name: String = ""
@export var texture_name: String = ""

# Called when the node enters the scene tree for the first time.
func _ready():
	var cell = TextureLoader.FindCell(sprite_name, texture_name)
	self.texture = cell.GetTexture()
	self.region_enabled = true
	self.region_rect = Rect2(cell.X, cell.Y, cell.W, cell.H)

# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	pass
