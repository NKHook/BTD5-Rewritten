//
// Created by mike on 4/25/24.
//

#ifndef ACTOR_COMPONENTS_H
#define ACTOR_COMPONENTS_H

#include "constants.h"

#include <godot_cpp/variant/vector2.hpp>

#include <array>
#include <bitset>

namespace rewritten {
    struct PositionComponent {
        std::array<godot::Vector2, MAX_KEY_FRAMES> mPositions{};
        std::array<float, MAX_KEY_FRAMES> mTimes{};
        std::bitset<MAX_KEY_FRAMES> mUsedFramesMask{};

        [[nodiscard]] auto used_frame_count() const
        {
            return mUsedFramesMask.count();
        }

        [[nodiscard]] auto is_frame_used(const size_t frame) const {
            return mUsedFramesMask.test(frame);
        }

        auto get_min_frame(float time) const {

            std::ranges::find_if(mTimes, [time](auto t){ return t > time; });
        }
    };
}

#endif //ACTOR_COMPONENTS_H
