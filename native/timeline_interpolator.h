//
// Created by mike on 4/11/24.
//

#ifndef TIMELINE_INTERPOLATOR_H
#define TIMELINE_INTERPOLATOR_H

#include <entt/entt.hpp>

namespace rewritten {
    struct TimelineInterpolator {
        TimelineInterpolator(entt::registry& registry);
    private:
        entt::registry& actor_registry;
    };
}

#endif //TIMELINE_INTERPOLATOR_H
