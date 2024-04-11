extends DebugLabel

# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(_delta):
	self.text = "FPS " + String.num(Engine.get_frames_per_second())
	pass
