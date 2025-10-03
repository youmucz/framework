@tool
extends EditorPlugin

const TIMER_SINGLETON = "TimerManager"
const COROUTINE_SINGLETON = "CoroutineManager"



func _enter_tree() -> void:
	add_autoload_singleton(COROUTINE_SINGLETON, "res://addons/framework/utility/hierarchical_coroutines/coroutine_manager.tscn")
	add_autoload_singleton(TIMER_SINGLETON, "res://addons/framework/utility/timer/timer_manager.tscn")


func _exit_tree() -> void:
	remove_autoload_singleton(COROUTINE_SINGLETON)
	remove_autoload_singleton(TIMER_SINGLETON)
