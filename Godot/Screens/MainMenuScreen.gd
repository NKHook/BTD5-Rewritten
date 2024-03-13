extends Node2D

@export var intro_duration: float = 10.0
var screen_manager: Node2D = null

const compound_sprite_script = preload("res://Godot/Scripts/compound_sprite.gd")

var time_passed: float = 0.0
var buildings: Array[Building] = []

func get_building_sprites(buildingJson: Variant) -> Array[Building]:
	var result: Array[Building] = []
	for child in $menu.get_children():
		if child.get_script() == compound_sprite_script:
			for building in buildingJson:
				if child.sprite_definition_res.ends_with(building["SpriteFile"]):
					child.set_script(Building)
					child.sprite_definition_res = "Assets/JSON/Tablet/UILayout/" + building["SpriteFile"]
					child.screen = building["Screen"]
					child.loc_name = building["Name"]
					child.initialize()
					result.push_back(child)
	return result
	
func get_building_screens(buildingJson: Variant) -> Array[String]:
	var result: Array[String] = []
	for building in buildingJson:
		result.push_back(building["Screen"])
	return result
	
# Called when the node enters the scene tree for the first time.
func _ready():
	screen_manager = find_parent("screen_manager")
	var buildingsJson = JetFileImporter.GetJsonEntry("Assets/JSON/ScreenDefinitions/MainMenu/BuildingsNoSocial.json")
	buildings = get_building_sprites(buildingsJson["Buildings"])
	for building in buildings:
		var collider = Area2D.new()
		var area = CollisionShape2D.new()
		area.name = "interaction_check"
		area.shape = CircleShape2D.new()
		area.shape.radius = 250.0
		collider.add_child(area)
		collider.mouse_entered.connect(_building_mouse_entered.bind(building))
		collider.mouse_exited.connect(_building_mouse_exited.bind(building))
		building.add_child(collider)

# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	time_passed += delta;
	if time_passed > intro_duration:
		var camera_follow = $camera_path/camera_follow as PathFollow2D
		camera_follow.progress_ratio += delta
		if camera_follow.progress_ratio > 1.0:
			$sky.queue_free()
			$intro.queue_free()
		
	if Input.is_mouse_button_pressed(MOUSE_BUTTON_LEFT):
		time_passed = intro_duration
		
		for building in buildings:
			if building.hovered:
				print(building.screen)
				var popup_screen = load("res://Godot/Screens/" + building.screen + ".tscn")
				screen_manager.add_child(popup_screen.instantiate())
				

func _building_mouse_entered(building: Building):
	building.hovered = true
	
func _building_mouse_exited(building: Building):
	building.hovered = false
