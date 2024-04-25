//
// Created by mike on 4/11/24.
//

#include "register_types.h"

#include "compound_sprite.h"

#include <gdextension_interface.h>
#include <godot_cpp/core/defs.hpp>
#include <godot_cpp/godot.hpp>

using namespace godot;

void godot::initialize_native_module(ModuleInitializationLevel level) {
    if(level != MODULE_INITIALIZATION_LEVEL_SCENE)
        return;

    ClassDB::register_class<CompoundSprite>();
}

void godot::uninitialize_native_module(ModuleInitializationLevel level) {
    if (level != MODULE_INITIALIZATION_LEVEL_SCENE)
        return;
}

extern "C" {
    //Initialization
    GDExtensionBool GDE_EXPORT rewritten_native_init(GDExtensionInterfaceGetProcAddress p_get_proc_address, const GDExtensionClassLibraryPtr p_library, GDExtensionInitialization* p_initialization) {
        GDExtensionBinding::InitObject init_obj(p_get_proc_address, p_library, p_initialization);

        init_obj.register_initializer(initialize_native_module);
        init_obj.register_terminator(uninitialize_native_module);
        init_obj.set_minimum_library_initialization_level(MODULE_INITIALIZATION_LEVEL_SCENE);

        return init_obj.init();
    }
}