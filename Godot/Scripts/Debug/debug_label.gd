extends Label

class_name DebugLabel

# Called when the node enters the scene tree for the first time.
func _ready():
	var debug_font = FontFile.new()
	debug_font.load_bitmap_font(AssetImporterConfig.fonts_dir + "/Ultra/debugfont.fnt")
	add_theme_font_override("font", debug_font)
	
	position.y = get_index() * 25


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	pass
