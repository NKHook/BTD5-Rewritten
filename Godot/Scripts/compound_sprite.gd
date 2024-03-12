extends Node

class_name CompoundSprite

@export var sprite_definition_res: String;
@export var animating: bool = true

const shader = preload("res://Godot/Shaders/compound_sprite.tres")

enum AlignmentValues { Default, MinX, MaxX, MinY, MaxY, Unknown3 }
enum ActorTypes { Invalid, Sprite, CompoundSprite }
enum FlipValues { Default, Horizontal, Vertical, Both }
var timeline: TimelineInterpolator = null
var used_cells: Array[Variant] = [] #TODO: THIS WAS CELL ENTRY NOT VARIANT
var child_uids: Array[int] = []
var initial_states: Array[ActorState] = []
var child_cells: Array[Variant] = [] #TODO: THIS WAS CELL ENTRY NOT VARIANT

class ActorState:
	# Necessary to store for alignment changes
	var cell: Variant = null #TODO: THIS WAS CELL ENTRY NOT VARIANT
	
	# Normal state stuff
	var alignment: Array[AlignmentValues]
	var alpha: float = 1.0
	var angle: float = 0.0
	var color: Color = Color.WHITE
	var flip: FlipValues = FlipValues.Default
	var position: Vector2
	var scale: Vector2 = Vector2(1.0, 1.0)
	var shown: bool = true
	var time: float = 0.0
	
	func _init(cell: Variant, actor: Variant = null): #TODO: CELL WAS CELL ENTRY NOT VARIANT
		self.cell = cell
		if actor == null:
			return
		
		self.alignment = [AlignmentValues.values()[actor["Alignment"][0]], AlignmentValues.values()[actor["Alignment"][1]]]
		self.alpha = actor["Alpha"]
		self.angle = actor["Angle"]
		if actor.has("Colour"):
			var argb = Color.hex(actor["Colour"])
			#Re order the color channels. BTD5 uses ARGB while godot uses RGBA
			self.color.a = argb.r
			self.color.r = argb.g
			self.color.b = argb.b
			self.color.g = argb.a
		self.flip = FlipValues.values()[actor["Flip"]]
		self.position = Vector2(actor["Position"][0], actor["Position"][1])
		self.scale = Vector2(actor["Scale"][0], actor["Scale"][1])
		self.shown = actor["Shown"]
		
		if actor.has("Time"):
			self.time = actor["Time"]
		
	func apply(node: Node2D):
		if node is Sprite2D:
			if cell != null:
				var center_point = Vector2(cell.Aw, cell.Ah) * 0.5
				node.centered = false
				match alignment[0]:
					AlignmentValues.Default:
						node.offset.x = cell.Ax
					AlignmentValues.MinX:
						node.offset.x = cell.Ax + (cell.Aw * 0.5)
					AlignmentValues.MaxX:
						node.offset.x = cell.Ax - (cell.Aw * 0.5)
				match alignment[1]:
					AlignmentValues.Default:
						node.offset.y = cell.Ay
					AlignmentValues.MinY:
						node.offset.y = cell.Ay + (cell.Ah * 0.5)
					AlignmentValues.MaxY:
						node.offset.y = cell.Ay - (cell.Ah * 0.5)
				node.offset -= center_point
		
		if node.material == null:
			node.material = ShaderMaterial.new()
			node.material.shader = shader
		node.material.set_shader_parameter("color", self.color)
	
		node.rotation_degrees = self.angle
		node.scale = self.scale
		node.visible = self.shown
		node.position = self.position * 4
		
		node.scale.x *= -1.0 if self.flip == FlipValues.Horizontal || self.flip == FlipValues.Both else 1.0
		node.scale.y *= -1.0 if self.flip == FlipValues.Vertical || self.flip == FlipValues.Both else 1.0
		
	func interpolate(other: ActorState, delta: float) -> ActorState:
		if self.cell == null || other.cell == null:
			assert(self.cell == other.cell)
		else:
			assert(self.cell.name == other.cell.name)
		var result: ActorState = ActorState.new(self.cell)
		result.alignment = self.alignment
		result.alpha = lerpf(self.alpha, other.alpha, delta)
		result.angle = lerpf(self.angle, other.angle, delta)
		result.color = lerp(self.color, other.color, delta)
		result.flip = self.flip
		result.position = lerp(self.position, other.position, delta)
		result.scale = lerp(self.scale, other.scale, delta)
		result.shown = self.shown# if delta < 0.5 else other.shown
		result.time = lerpf(self.time, other.time, delta)
		return result

class TimelineInterpolator:
	var time: float = 0.0
	var length: float = 0.0
	var loop: bool = true
	var uids: Array[int] = []
	var nodes: Array[Node2D] = []
	var states: Array[ActorState] = []
	var states_length: Array[int] = []
	
	func _init(length: float):
		self.length = length
	
	func tick(delta: float) -> bool:
		self.time += delta
		if self.time >= self.length and loop:
			self.time -= self.time
		return self.time >= self.length;
	
	func add_timeline(uid: int, node: Node2D, states: Array[ActorState]):
		self.uids.push_back(uid)
		self.nodes.push_back(node)
		states.sort_custom(func(x: ActorState, y: ActorState):
			return x.time < y.time
		)
		self.states.append_array(states)
		self.states_length.push_back(states.size())
		
	func start_state_idx_for_uid(uid: int) -> int:
		var index = self.uids.find(uid)
		var result: int = 0
		for i in range(0, index):
			result += self.states_length[i]
		return result
		
	func end_state_idx_for_uid(uid: int) -> int:
		var index = self.uids.find(uid)
		var result: int = 0
		for i in range(0, index + 1):
			result += self.states_length[i]
		return result
		
	func get_state_for_uid(uid: int) -> ActorState:
		if states.is_empty():
			return null

		var begin = start_state_idx_for_uid(uid)
		var end = end_state_idx_for_uid(uid)

		if begin == end:
			return null

		var previous_idx = begin
		var next_idx = begin + 1

		# Find the indices of the previous and next states
		for i in range(begin + 1, end):
			if states[i].time <= time:
				previous_idx = i
			else:
				next_idx = i
				break

		var previous = states[previous_idx]
		if next_idx >= end:
			return previous
		var next = states[next_idx]

		if next.time < previous.time:
			return states[end - 1]

		# Check if we need to interpolate or return one of the states directly
		if time <= previous.time:
			return previous
		elif time >= next.time:
			return next

		# Interpolate between the previous and next states
		var lerp_factor = (time - previous.time) / (next.time - previous.time)
		return previous.interpolate(next, lerp_factor)

func load_compound_sprite(sprite: String) -> Node2D:
	var compound_sprite = Node2D.new()
	compound_sprite.set_script(get_script())
	compound_sprite.sprite_definition_res = PathUtil.get_parent_path(sprite_definition_res) + "/" + sprite;
	compound_sprite.animating = animating
	return compound_sprite

func load_single_sprite(cell: Variant, state: ActorState) -> Sprite2D: #TODO: CELL WAS CELL ENTRY NOT VARIANT
	# Search the global sprite_table for the sprite
	var sprite_obj = Sprite2D.new()
	sprite_obj.texture = cell.GetTexture()
	sprite_obj.region_enabled = true;
	sprite_obj.region_rect = Rect2(cell.X, cell.Y, cell.W, cell.H)
	state.apply(sprite_obj)
	return sprite_obj

func load_actor(actor: Variant) -> Node2D:
	var sprite: String = actor["sprite"]
	var type: ActorTypes = ActorTypes.values()[actor["type"]]
	var uid: int = actor["uid"]
	
	child_uids.push_back(uid)
	
	var result = null
	match type:
		ActorTypes.Sprite:
			var cell: Variant = null
			for used in used_cells:
				if used.Name == sprite:
					cell = used
			assert(cell != null)
			
			var state: ActorState = ActorState.new(cell, actor)
			initial_states.push_back(state)
			
			child_cells.push_back(cell)
	
			result = load_single_sprite(cell, state)
			
		ActorTypes.CompoundSprite:
			result = load_compound_sprite(sprite)
			initial_states.push_back(ActorState.new(null, actor))
			child_cells.push_back(null)
			
	
	result.name = String.num_int64(uid)
	return result

func load_sprite_info(stage_options: Variant) -> Array[Variant]: #TODO: ARRRAY OF CELL ENTRY NOT VARIANT
	var result: Array[Variant] = [] # SAME HERE
	
	var infos_json = stage_options["SpriteInfo"]
	for info in infos_json:
		var sprite: String = info["SpriteInfo"]
		var texture: String = info["Texture"]
		
		assert(TextureLoader.has_method("FindCell"))
		var cell = TextureLoader.FindCell(sprite, texture)
		if cell != null:
			result.push_back(cell)
		
	return result
	
func load_stage_options(stage_options: Variant) -> TimelineInterpolator:
	var duration: float = stage_options["StageLength"]
	return TimelineInterpolator.new(duration)

# Called when the node enters the scene tree for the first time.
func _ready():
	assert(sprite_definition_res != "")
	var sprite_definition_json: Variant = JetFileImporter.GetJsonEntry(sprite_definition_res)
		
	var stage_options = sprite_definition_json["stageOptions"]
	used_cells = load_sprite_info(stage_options)
	timeline = load_stage_options(stage_options)
	
	var actors = sprite_definition_json["actors"]
	for actor in actors:
		var sprite_obj = load_actor(actor)
		if sprite_obj != null:
			add_child(sprite_obj)
	
	var timelines_json = sprite_definition_json["timelines"]
	for timeline_json in timelines_json:
		var uid: int = timeline_json["spriteuid"]
		var stages_json = timeline_json["stage"]
		if stages_json == null:
			continue
		
		var stages: Array[ActorState] = []
		for stage_json in stages_json:
			if stage_json == null:
				continue
			
			# Prevent states with the same time overwriting eachother
			# the game only uses the first one at the same time for some reason
			var time = stage_json["Time"]
			if not stages.filter(func (stage):
				return stage.time == time
			).is_empty():
				continue
			
			var index: int = child_uids.find(uid)
			var cell: Variant = child_cells[index] #TODO: CELL WAS CELL ENTRY NOT VARIANT
			stages.push_back(ActorState.new(cell, stage_json))
			
		var node: Node2D = null
		for child in get_children(false):
			if child.name.to_int() == uid:
				node = child
		assert(node != null)
		
		timeline.add_timeline(uid, node, stages)


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	if animating:
		var visible = not timeline.tick(delta)
		for child in get_children(false):
			child.visible = visible;
			var uid: int = child.name.to_int()
			var state: ActorState = timeline.get_state_for_uid(uid)
			if state == null:
				var index = child_uids.find(uid)
				state = initial_states[index]
				assert(state != null)
			state.apply(child)
