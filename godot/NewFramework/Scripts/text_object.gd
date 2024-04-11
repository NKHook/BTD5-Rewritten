extends Label

class_name TextObject

enum FontQuality { Ultra, High, Low }

@export var font_name: String = ""
@export var font_quality: FontQuality = FontQuality.Ultra

# Called when the node enters the scene tree for the first time.
func _ready():
	var text_font = FontFile.new()
	var font_file: String = AssetImporterConfig.fonts_dir + "/" + FontQuality.keys()[font_quality] + "/" + font_name + ".fnt";
	assert(Error.OK == text_font.load_bitmap_font(font_file))
	add_theme_font_override("font", text_font)

# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	pass
