//
// Created by mike on 4/11/24.
//

#ifndef ACTOR_H
#define ACTOR_H

#include <array>

#include "godot_cpp/variant/color.hpp"
#include "godot_cpp/variant/variant.hpp"
#include "godot_cpp/variant/vector2.hpp"

namespace rewritten {
    enum struct ActorAlignment
    {
        Default,
        MinX,
        MaxX,
        MinY,
        MaxY,
        Unknown3
    };

    enum struct ActorFlip
    {
        Default,
        Horizontal,
        Vertical,
        Both
    };

    enum struct TextureQuality
    {
        Invalid = 0,
        Low = 1,
        Mobile = 2,
        Tablet = 3,
        Ultra = 4
    };

    struct NativeCellEntry {
        godot::String cell_name{};
        int x{};
        int y{};
        int w{};
        int h{};
        int ax{};
        int ay{};
        int aw{};
        int ah{};

        explicit NativeCellEntry(const godot::Variant& cell_variant);

    private:
        const godot::Variant& underlying_cell{};
    };

    struct ActorState {

    private:
        godot::Variant cell_entry;

        std::array<ActorAlignment, 2> alignment{};
        float alpha = 1.0f;
        float angle = 0.0f;
        godot::Color color = godot::Color::hex(0xFFFFFFFF);
        ActorFlip flip = ActorFlip::Default;
        godot::Vector2 position;
        godot::Vector2 scale{ 1.0f, 1.0f };
        bool shown = true;
        float time = 0.0f;
    };
}

#endif //ACTOR_H
