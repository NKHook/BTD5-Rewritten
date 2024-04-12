include(FetchContent)
cmake_minimum_required(VERSION 3.26)
project(rewritten-native)

set(CMAKE_CXX_STANDARD 23)

add_library(rewritten-native SHARED
        compound_sprite.cpp
        register_types.cpp
        timeline_interpolator.cpp
        actor.h
        actor.cpp
)

FetchContent_Declare(
        gdcpp
        GIT_REPOSITORY https://github.com/godotengine/godot-cpp.git
        GIT_TAG godot-4.2.1-stable
)
FetchContent_GetProperties(gdcpp)
if(NOT gdcpp_POPULATED)
    FetchContent_Populate(gdcpp)
    add_subdirectory(${gdcpp_SOURCE_DIR} ${gdcpp_BINARY_DIR})
endif()

FetchContent_Declare(
        entt
        GIT_REPOSITORY https://github.com/skypjack/entt.git
        GIT_TAG v3.13.2
)
FetchContent_GetProperties(entt)
if(NOT entt_POPULATED)
    FetchContent_Populate(entt)
    add_subdirectory(${entt_SOURCE_DIR} ${entt_BINARY_DIR})
endif()

target_link_libraries(rewritten-native PRIVATE godot-cpp EnTT)