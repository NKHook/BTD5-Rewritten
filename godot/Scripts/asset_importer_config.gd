extends Node

var game_dir: String = ""
var assets_dir: String = ""
var fonts_dir: String = ""
var jet_file: String = ""

# Called when the node enters the scene tree for the first time.
func _ready():
	var executable_path: String = OS.get_executable_path()
	game_dir = PathUtil.get_parent_path(executable_path)
	assets_dir = game_dir + "/Assets"
	
	if not FileAccess.file_exists(assets_dir):
		#game_dir = "D:/SteamLibrary/steamapps/common/BloonsTD5"
		game_dir = "/Users/mike/Library/Application Support/Steam/steamapps/common/BloonsTD5/btd5steam.app/Contents/Resources"
		#HEY!!! You probably see this isnt working for you!!
		#CHANGE THE PATH ABOVE to the path of *YOUR OWN BTD5 FOLDER*!!!
		#This is the path where the BTD5 exe file is located
		#This is done so you can have the godot editor installed anywhere
		#But the exe is expected to be in the game's install folder
		#You also may need to replace the \ with / instead
		assert(DirAccess.dir_exists_absolute(game_dir))
		
	assets_dir = game_dir + "/Assets"
	fonts_dir = assets_dir + "/Fonts"
	jet_file = assets_dir + "/BTD5.jet"
	print("Assets Dir (GDScript): ", assets_dir)
