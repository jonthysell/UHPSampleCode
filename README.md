# UHP Sample Code #

This repo contains code samples which implement the [Universal Hive Protocol](https://github.com/jonthysell/Mzinga/wiki/UniversalHiveProtocol) or UHP.

The UHP is a set of specifications for building software for playing the board game [Hive](http://hivegame.com/). The UHP is designed to facilitate software interoperability in order to build a community of developers who create Hive-playing AIs.

## Engines ##

In the UHP, an "engine" is responsible for implementing all the logic necessary to play a game of Hive, like keeping track of pieces on the board and calculating the set of valid moves, and letting players play valid moves. An engine is also where developers implement their AI, to calculate the next "best move" for a board.

This repo contains several sample engines written in different programming languages. Each represents a minimal implementation of a UHP engine - just enough code to play the base game of Hive without expansion pieces. They have no AI per se, so when asked for a "best move" they simply return the first valid move they have calculated.

These sample engines are provided as starting points for would-be Hive developers. Each has their own readme with specific build instructions. Use them to bootstrap your own UHP engine, whether by building on the existing code or simply using it to inspire your own implementation.

## Copyright ##

Hive Copyright (c) 2016 Gen42 Games. This repo is in no way associated with or endorsed by Gen42 Games.
