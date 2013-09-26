Tychaia
=======

An openly developed, procedurally generated infinite RPG.

*This game is __not__ open source software!*  While it is openly developed, it is not a freeware game.  We're still in early stages of development, so we have yet to write up an appropriate license.

Links
-------

  * [Tychaia on TIGSource](http://forums.tigsource.com/index.php?topic=27727)
  * [Tychaia Website](http://www.tychaia.com/)
  * [Make me a World!](http://makemeaworld.com/)

About
--------

Imagine a game where the world never ends, the scale is infinite and there's always one more quest.  Where the game starts with nothing and ends with your death.  Welcome to Tychaia.

![A very early screenshot.](http://i.imgur.com/vD8MjHG.png)

The game places a very heavy focus on procedural generation; it generates the entire world from scratch every time you play, including content such as:

  * Terrain
  * Dungeons
  * Regions
  * Towns
  * Items
  * Enemies
  * NPC and Family Tree Generation
  * Backstory Generation
  * Quest Generation

![The world generation design tool.](http://i.imgur.com/kyd5A.png)

Building
------------------------

If you've got a license to the game, you can build it using Mono or .NET in the following manner:

  1. Goto the `Build` directory and run `xbuild /p:TargetPlatform=<Platform>`, where `<Platform>` is either "Linux" or "Windows".
  2. Use the resulting `Tychaia.<Platform>.sln` file to build the game.

