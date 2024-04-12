//
// Created by mike on 4/11/24.
//

#ifndef ACTOR_H
#define ACTOR_H

#include <array>

#include "godot_cpp/godot.hpp"
#include "godot_cpp/variant/color.hpp"
#include "godot_cpp/variant/variant.hpp"
#include "godot_cpp/variant/vector2.hpp"

namespace rewritten {
    enum struct actor_alignment
    {
        Default,
        MinX,
        MaxX,
        MinY,
        MaxY,
        Unknown3
    };

    enum struct actor_flip
    {
        Default,
        Horizontal,
        Vertical,
        Both
    };

    enum struct texture_quality
    {
        Invalid = 0,
        Low = 1,
        Mobile = 2,
        Tablet = 3,
        Ultra = 4
    };

    struct native_cell_entry {
        godot::String cell_name{};
        int x{};
        int y{};
        int w{};
        int h{};
        int ax{};
        int ay{};
        int aw{};
        int ah{};

        explicit native_cell_entry(const godot::Variant& cell_variant) : underlying_cell(cell_variant) {
            x = cell_variant.get("X");
            y = cell_variant.get("Y");
            w = cell_variant.get("W");
            h = cell_variant.get("H");
            ax = cell_variant.get("Ax");
            ay = cell_variant.get("Ay");
            aw = cell_variant.get("Aw");
            ah = cell_variant.get("Ah");

            cell_name = cell_variant.get("CellName");
        }

    private:
        const godot::Variant& underlying_cell{};
    };

    struct actor_state {

    private:
        godot::Variant cell_entry;

        std::array<actor_alignment, 2> alignment{};
        auto alpha = 1.0f;
        auto angle = 0.0f;
        auto color = godot::Color::hex(0xFFFFFFFF);
        auto flip = actor_flip::Default;
        auto position = godot::Vector2(0.0f,0.0f);
        auto scale = godot::Vector2(0.0f,0.0f);
        bool shown = true;
        float time = 0.0f;
    };
}

#endif //ACTOR_H
