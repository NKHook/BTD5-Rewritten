extends DebugLabel

# Called every frame. 'delta' is the elapsed time since the previous frame.
func _process(delta):
	self.text = String.num(get_tree().get_node_count()) + " total nodes"
