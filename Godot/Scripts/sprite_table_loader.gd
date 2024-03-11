extends Node

enum TextureQuality { Tablet, Ultra }
enum TextureType { INVALID, PNG, JPG, JPNG }

@export var textures_dir_path: String = "res://Assets/Textures/";
@export var texture_quality: String = "Ultra";
var sprite_infos: Array[SpriteInfo] = [];

class CellEntry:
	var parent: Variant
	var textures_dir_path: String
	var file_path: String
	var quality: TextureQuality = TextureQuality.Ultra
	var name: String = ""
	var x: int = 0
	var y: int = 0
	var w: int = 0
	var h: int = 0
	var ax: int = 0
	var ay: int = 0
	var aw: int = 0
	var ah: int = 0
	
	func _init(parent: Variant, textures_dir_path: String, file_path: String,
				quality: TextureQuality, name: String, x, y, w, h, ax, ay, aw, ah):
		self.parent = parent
		self.textures_dir_path = textures_dir_path
		self.file_path = file_path
		self.quality = quality
		self.name = name
		
		self.x = int(x)
		self.y = int(y)
		self.w = int(w)
		self.h = int(h)
		self.ax = int(ax)
		self.ay = int(ay)
		self.aw = int(aw)
		self.ah = int(ah)

class AnimationEntry:
	var parent: FrameInfo
	var textures_dir_path: String
	var file_path: String
	var quality: TextureQuality = TextureQuality.Ultra
	var name: String = ""
	
	var cells: Array[CellEntry] = []
	
	func _init(parent: FrameInfo, textures_dir_path: String, file_path: String,
				quality: TextureQuality, name: String):
		self.parent = parent
		self.textures_dir_path = textures_dir_path
		self.file_path = file_path
		self.quality = quality
		self.name = name
		
	func add_cell(entry: CellEntry):
		self.cells.push_back(entry)
		
	func get_cell(name: String) -> CellEntry:
		for entry in self.cells:
			if entry.name == name:
				return entry
		return null
		
	func find_cell(name: String) -> CellEntry:
		for cell in self.cells:
			if cell.name == name:
				return cell
		return null
		
class FrameInfo:
	var parent: SpriteInfo
	var textures_dir_path: String
	var file_path: String
	var quality: TextureQuality = TextureQuality.Ultra
	var name: String = ""
	var texw: int = 0
	var texh: int = 0
	var type: TextureType = TextureType.PNG
	
	var animations: Array[AnimationEntry] = []
	var cells: Array[CellEntry] = []
	
	func _init(parent: SpriteInfo, textures_dir_path: String, file_path: String, quality: TextureQuality, name: String, texw: String, texh: String, type: String):
		self.parent = parent
		self.textures_dir_path = textures_dir_path
		self.file_path = file_path
		self.quality = quality
		self.name = name
		self.texw = int(texw)
		self.texh = int(texh)
		self.type = TextureType.get(type.to_upper())
		
	func add_animation(entry: AnimationEntry):
		self.animations.push_back(entry)
	
	func add_cell(entry: CellEntry):
		self.cells.push_back(entry)
		
	func get_animation(name: String) -> AnimationEntry:
		for entry in self.animations:
			if entry.name == name:
				return entry
		return null
		
	func get_cell(name: String) -> CellEntry:
		for entry in self.cells:
			if entry.name == name:
				return entry
		return null
		
	func find_cell(name: String) -> CellEntry:
		for animation in self.animations:
			var result: CellEntry = animation.find_cell(name)
			if result != null:
				return result
		for cell in self.cells:
			if cell.name == name:
				return cell
		return null

class SpriteInfo:
	var name: String = ""
	var parent: SpriteInfo
	var path: String = ""
	var children: Array[SpriteInfo] = []
	var quality: TextureQuality = TextureQuality.Ultra
	var frames: Array[FrameInfo] = []
	
	func _init(name: String, textures_dir_path: String, file_path: String, quality: TextureQuality = TextureQuality.Ultra, parent: SpriteInfo = null):
		self.name = name
		self.parent = parent
		self.path = file_path
		self.quality = quality
		
		var parser = XMLParser.new()
		parser.open(file_path)
		var within_info: bool = false
		var current_frame: FrameInfo = null
		var current_animation: AnimationEntry = null
		while parser.read() != ERR_FILE_EOF:
			if parser.get_node_type() == XMLParser.NODE_ELEMENT_END:
				var node_name = parser.get_node_name()
				if "SpriteInformation" == node_name:
					within_info = false
				if "FrameInformation" == node_name:
					assert(current_frame != null)
					self.frames.push_back(current_frame)
					current_frame = null
				if "Animation" == node_name:
					assert(current_animation != null)
					current_frame.add_animation(current_animation)
					current_animation = null
				continue
			if parser.get_node_type() == XMLParser.NODE_ELEMENT:
				var node_name = parser.get_node_name()
				if "SpriteInformation" == node_name:
					within_info = true
					continue
				
				var attributes_dict = {}
				for idx in range(parser.get_attribute_count()):
					attributes_dict[parser.get_attribute_name(idx)] = parser.get_attribute_value(idx)
				
				if current_frame != null:
					if "Animation" == node_name:
						var animation_name = attributes_dict["name"]
						current_animation = AnimationEntry.new(current_frame, textures_dir_path, file_path, quality, animation_name)
						
					if "Cell" == node_name:
						var cell_name = attributes_dict["name"]
						var cell_x = attributes_dict["x"]
						var cell_y = attributes_dict["y"]
						var cell_w = attributes_dict["w"]
						var cell_h = attributes_dict["h"]
						var cell_ax = attributes_dict["ax"]
						var cell_ay = attributes_dict["ay"]
						var cell_aw = attributes_dict["aw"]
						var cell_ah = attributes_dict["ah"]
						
						var the_cell: CellEntry = CellEntry.new(null, textures_dir_path, file_path, quality, cell_name, cell_x, cell_y, cell_w, cell_h, cell_ax, cell_ay, cell_aw, cell_ah)
						if current_animation != null:
							the_cell.parent = current_animation
							current_animation.add_cell(the_cell)
						else:
							the_cell.parent = current_frame
							current_frame.add_cell(the_cell)
				elif within_info:
					if "SpriteInfoXml" == node_name:
						var sheet_name = attributes_dict["name"]
						var sheet_type = attributes_dict["type"]
						var actual_sheet_type: TextureType = TextureType.INVALID
						if "png" == sheet_type:
							actual_sheet_type = TextureType.PNG
						if "jpg" == sheet_type:
							actual_sheet_type = TextureType.JPG
						if "jpng" == sheet_type:
							actual_sheet_type = TextureType.JPNG
						assert(actual_sheet_type != TextureType.INVALID)
						self.children.push_back(SpriteInfo.new(sheet_name, textures_dir_path, textures_dir_path + "/" + TextureQuality.keys()[quality] + "/" + sheet_name + ".xml", quality, self))
					if "FrameInformation" == node_name:
						var frame_name = attributes_dict["name"]
						var texw = attributes_dict["texw"]
						var texh = attributes_dict["texh"]
						var type = attributes_dict["type"]
						current_frame = FrameInfo.new(self, textures_dir_path, file_path, quality, frame_name, texw, texh, type)
	
	func get_child_info(name: String) -> SpriteInfo:
		for info in self.children:
			if info.path.ends_with(name + ".xml"):
				return info
		return null
		
	func get_frame_info(name: String) -> FrameInfo:
		for info in self.frames:
			if info.name == name:
				return info
		return null
		
	func find_cell(name: String, texture: String = "") -> CellEntry:
		for info in self.children:
			var result: CellEntry = info.find_cell(name, texture)
			if result != null:
				return result
		for frame in self.frames:
			if texture != "" and frame.name != texture:
				continue
			var result: CellEntry = frame.find_cell(name)
			if result != null:
				return result
		return null
		
	func find_frame(name: String) -> FrameInfo:
		for info in self.children:
			var result: FrameInfo = info.find_frame(name)
			if result != null:
				return result
		for frame in self.frames:
			if frame.name == name:
				return frame
		return null


func load_sprites(textures_dir: DirAccess) -> Array[SpriteInfo]:
	var result: Array[SpriteInfo];
	var dir_path = textures_dir.get_current_dir()
	
	textures_dir.list_dir_begin()
	var filename = textures_dir.get_next()
	while filename != "":
		if not textures_dir.current_is_dir():
			result.push_back(SpriteInfo.new(filename.replace(".xml", ""), textures_dir_path, dir_path + "/" + filename))
		filename = textures_dir.get_next()
	return result

# Called when the node enters the scene tree for the first time.
func _ready():
	var textures_dir = DirAccess.open(textures_dir_path)
	sprite_infos = load_sprites(textures_dir)
	pass # Replace with function body.

func get_sprite_info(name: String) -> SpriteInfo:
	for info in sprite_infos:
		if info.path.ends_with(name + ".xml"):
			return info
	return null
	
func find_cell(name: String, texture: String = "") -> CellEntry:
	for info in sprite_infos:
		var result: CellEntry = info.find_cell(name, texture)
		if result != null:
			return result
	return null
	
func find_frame(name: String) -> FrameInfo:
	for info in sprite_infos:
		var result: FrameInfo = info.find_frame(name)
		if result != null:
			return result
	return null

func load_sprite(cell: CellEntry) -> Resource:
	var textures: DirAccess = DirAccess.open(cell.textures_dir_path)
	assert(textures != null)
	var animation: AnimationEntry = null
	var texture: FrameInfo = null
	if cell.parent is FrameInfo:
		texture = cell.parent
	elif cell.parent is AnimationEntry:
		animation = cell.parent
		texture = animation.parent
	var sprite_info: SpriteInfo = texture.parent
	var root_info: SpriteInfo = null
	if sprite_info.parent != null:
		root_info = sprite_info.parent
	assert(root_info != null)
	var sprite_filename: String = cell.name + ".png"
	var image_dir: String = cell.textures_dir_path + "/Extracted/" + root_info.name + "/" + sprite_info.name
	if animation != null:
		image_dir += "/" + animation.name
	var image_path: String = image_dir + "/" + cell.name + ".png"
	assert(image_path != "")
	
	return load(image_path)

# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	pass
