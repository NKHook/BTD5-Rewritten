//
// Created by mike on 4/11/24.
//

#ifndef COMPOUNDSPRITE_H
#define COMPOUNDSPRITE_H

#include <godot_cpp/classes/node2d.hpp>

#include "timeline_interpolator.h"

namespace godot {
    class CompoundSprite : public Node2D {
        GDCLASS(CompoundSprite, Node2D)

        String definition_path;
        bool animating = true;

        bool was_animating = false;
        rewritten::TimelineInterpolator timeline;
    public:
        CompoundSprite();
        void initialize();

        void _ready() override;
        void _process(double delta) override;

        void set_definition_path(const String& definition_path);
        const String& get_definition_path();

    protected:
        static void _bind_methods();
    };
}

#endif //COMPOUNDSPRITE_H
