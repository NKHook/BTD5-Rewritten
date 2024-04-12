//
// Created by mike on 4/11/24.
//

#include "compound_sprite.h"

#include <entt/entt.hpp>

using namespace godot;

static entt::registry actor_registry;

compound_sprite::compound_sprite() : timeline(actor_registry) {

}

void compound_sprite::initialize() {

}


void compound_sprite::_ready() {

}

void compound_sprite::_process(double delta) {

}

void compound_sprite::_bind_methods() {
    ClassDB::bind_method(D_METHOD("get_definition_path"), &compound_sprite::get_definition_path);
    ClassDB::bind_method(D_METHOD("set_definition_path", "definition_path"), &compound_sprite::set_definition_path);
    ClassDB::add_property("compound_sprite", PropertyInfo(Variant::STRING, "definition_path"), "set_definition_path", "get_definition_path");
}
