//
// Created by mike on 4/11/24.
//

#include "compound_sprite.h"

#include <entt/entt.hpp>

using namespace godot;

static entt::registry actor_registry;

CompoundSprite::CompoundSprite() : timeline(actor_registry) {

}

void CompoundSprite::initialize() {

}


void CompoundSprite::_ready() {

}

void CompoundSprite::_process(double delta) {

}

void CompoundSprite::_bind_methods() {
    ClassDB::bind_method(D_METHOD("get_definition_path"), &CompoundSprite::get_definition_path);
    ClassDB::bind_method(D_METHOD("set_definition_path", "definition_path"), &CompoundSprite::set_definition_path);
    ClassDB::add_property("CompoundSprite", PropertyInfo(Variant::STRING, "definition_path"), "set_definition_path", "get_definition_path");
}
