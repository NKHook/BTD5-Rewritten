//
// Created by mike on 4/11/24.
//

#include "actor.h"

using namespace rewritten;

NativeCellEntry::NativeCellEntry(const godot::Variant& cell_variant) : underlying_cell(cell_variant) {
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