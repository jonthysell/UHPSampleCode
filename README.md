# UHP Sample Code #

This repo contains code samples which implement the [Universal Hive Protocol](https://github.com/jonthysell/Mzinga/wiki/UniversalHiveProtocol) or UHP.

The UHP is a set of specifications for building software that play the board game [Hive](http://hivegame.com/). The UHP is designed to facilitate software interoperability among a community of developers who create Hive-playing AIs.

## Sample Engines ##

[![SampleEngine C++ CI](https://github.com/jonthysell/UHPSampleCode/actions/workflows/engines_cpp_ci.yml/badge.svg)](https://github.com/jonthysell/UHPSampleCode/actions/workflows/engines_cpp_ci.yml) [![SampleEngine C# CI](https://github.com/jonthysell/UHPSampleCode/actions/workflows/engines_cs_ci.yml/badge.svg)](https://github.com/jonthysell/UHPSampleCode/actions/workflows/engines_cs_ci.yml)

In the UHP, an "engine" is responsible for implementing all the logic necessary to play a game of Hive, like keeping track of pieces on the board, calculating the set of valid moves, and letting players play those moves. An engine is also where developers implement their AI in order to calculate the next "best move".

This repo contains sample engines written in different programming languages. Each represents a minimal implementation of a UHP engine - just enough code to fulfill the UHP specifications and play the base game of Hive without expansion pieces. They have no AI per se, so when asked for a "best move" they simply return a valid move they've calculated.

These sample engines are provided as starting points for would-be Hive developers. They are not optimized, or necessarily the best or only way to build a UHP engine. Use them to bootstrap your own UHP engine, whether by building directly on the existing code or just as inspiration your own implementation. Note, each has its own readme with specific build and setup instructions.

## Copyrights ##

All code in this repo is free and unencumbered software released into the public domain.

Hive Copyright (c) 2016 Gen42 Games. This repo is in no way associated with or endorsed by Gen42 Games.
