cmake_minimum_required(VERSION 3.16.0)

project(SampleEngine VERSION 1.0)

add_executable(SampleEngine)

target_include_directories(SampleEngine PRIVATE include)

target_sources(SampleEngine PRIVATE
    src/main.cpp
    src/Board.cpp
    src/Engine.cpp
    src/Enums.cpp
    src/Move.cpp
    src/Position.cpp
)
