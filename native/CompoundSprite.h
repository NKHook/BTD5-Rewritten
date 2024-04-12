//
// Created by mike on 4/11/24.
//

#ifndef COMPOUNDSPRITE_H
#define COMPOUNDSPRITE_H

#include <godot_cpp/classes/node2d.hpp>

namespace godot {
    class CompoundSprite : public Node2D {
        GDCLASS(CompoundSprite, Node2D)

    public:
        CompoundSprite();

        void _process(double delta) override;

    protected:
        static void _bind_methods();
    };
}

#endif //COMPOUNDSPRITE_H
