Unity Demo 1
============

I played around for a couple days making this little demo platformer in Unity; it was my first time really playing with Unity in any non-trivial capacity.

Here's a screenshot:

![Screenshot](https://github.com/petersn/unity-demo1/blob/master/screenshot.png)

Sprite Workflow
---------------

I made and animated all of image assets and characters in [Aseprite](https://www.aseprite.org/).

The version of Unity I am using (2017.30p2) has some really bad behavior of having the color of neighboring pixels bleed over at the edges of each tile in a sprite sheet.
To work around this my workflow was to save my `.aseprite` files into `Assets/Sprites`, export their sprite sheets into `Assets/Sprites/raw`, then write down the tile size of the sprite sheet into `Assets/Python/image-descs.json`, then rerun the script `Assets/Python/borderize.py`.
This script takes a sprite sheet whose cells are n by m pixels each and produces one whose cells are (n + 2) by (m + 2) pixels each, made by duplicating the outermost ring of pixels.
These processed sprite sheets get written into `Assets/Sprites/`
I would then import the processed sprite sheets into Unity, and use its nice offset features to pull out just the n by m sized cells ignoring the borders.
However, the simple presense of the border in the image texture eliminates the ugly color bleeding artifacts at the edges of sprites.

This script and work-flow is probably the only useful thing you can borrow from this repo.

Binaries
--------

Because this was a one-off project from a while ago that I'm putting on GitHub largely for my own personal archival purposes, I'm committing the faux pas of committing built binaries, because I don't expect to ever change or rebuild the project.
In `Builds/` you can find the game pre-build in [`LinuxBuild001.tar.bz2`](https://github.com/petersn/unity-demo1/raw/master/Builds/LinuxBuild001.tar.bz2) and [`WinBuild001.zip`](https://github.com/petersn/unity-demo1/raw/master/Builds/WinBuild001.zip).

License
-------

Everything is CC0/Public Domain, with the exception of `Assets/RippleEffect`, which is from [this GitHub repo](https://github.com/keijiro/RippleEffect).
Said project simply says "License: You can use the scripts and the shaders in this project as you like.", which I'm interpreting as being a relatively general release?
So, with the possible exception of the `Assets/RippleEffect` directory, you can do whatever you want with any of it.

