extends Node

var map_name: String;

# Called when the node enters the scene tree for the first time.
func _ready():
	var map_core_file = load("res://Assets/JSON/LevelDefinitions/"+map_name+"/"+map_name+".map")
	var map_props_file = load("res://Assets/JSON/LevelDefinitions/"+map_name+"/"+map_name+".map")
	var map_core = JSON.parse_string(map_core_file)
	var map_props = JSON.parse_string(map_props_file)


# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	pass
