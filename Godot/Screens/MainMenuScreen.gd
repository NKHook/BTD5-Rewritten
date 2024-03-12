extends Node2D

@export var intro_duration: float = 10.0

const compound_sprite_script = preload("res://Godot/Scripts/compound_sprite.gd")

var time_passed: float = 0.0
var buildings: Array[Node2D]

func get_building_sprites(buildingJson: Variant) -> Array[Node2D]:
	var result: Array[Node2D] = []
	for child in $menu.get_children():
		if child.get_script() == compound_sprite_script:
			for building in buildingJson:
				if child.sprite_definition_res.ends_with(building["SpriteFile"]):
					result.push_back(child)
	return result
	
# Called when the node enters the scene tree for the first time.
func _ready():
	var buildingsJson = JetFileImporter.GetJsonEntry("Assets/JSON/ScreenDefinitions/MainMenu/BuildingsNoSocial.json")
	buildings = get_building_sprites(buildingsJson["Buildings"])
	for building in buildings:
		var area = Area2D.new()
		area.name = "interaction_check"
		building.add_child(area)

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
