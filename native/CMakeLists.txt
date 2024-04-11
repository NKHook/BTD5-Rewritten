cmake_minimum_required(VERSION 3.26)
project(native)

set(CMAKE_CXX_STANDARD 23)

add_library(native SHARED library.cpp)
