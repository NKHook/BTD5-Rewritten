extends DebugLabel

# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	self.text = String.num(delta, 3) + " seconds/frame"
