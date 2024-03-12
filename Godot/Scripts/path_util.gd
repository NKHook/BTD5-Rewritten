extends Node

func walk(dir: DirAccess, callback: Callable):
	dir.list_dir_begin()
	var filename = dir.get_next()
	while filename != "":
		if dir.current_is_dir():
			var result = walk(DirAccess.open(dir.get_current_dir() + "/" + filename), callback)
			if result != null:
				return result
		else:
			var result = callback.call(dir.get_current_dir() + "/" + filename)
			if result != null:
				return result
		filename = dir.get_next()
	dir.list_dir_end()
	return null

func get_parent_path(path: String) -> String:
	var path_end = path.rfind("/")
	return path.substr(0, path_end)
