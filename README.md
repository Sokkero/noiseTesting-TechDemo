# Procedural generation using noise functions

## 1) General

This is a unity project without a standalone executable.<br>
The project can be directly added to unity, native version being 2020.1.2f1.<br>
The powerpoint presentation that was made for this showcase can be downloaded [here](https://www.dropbox.com/s/wh8s19lj9wke2gn/Noise-and-what-it-looks-like.pptx?dl=0).

## 2) Perlin noise

Perlin noise was the first type of gradient noise introduced to the world, and is what the presentation primarily covered. The main showcase of perlin noise and layering is found in `PerlinScene.unity`.<br>
The Terrain gameobject holds `GradientNoiseTerrainGenerator.cs`.
- A domain rotation toggle is included, which mitigates some of the algorithm's inherent square alignment artifacts.
- All values used in calculations can be accessed from the inspector.

## 2) Simplex(-type) noise

Simplex(-type) noise is a successor to Perlin that improves pattern angle distribution by changing the internal grid structure. A showcase with layering can be found in `SimplexScene.unity`.<br>
The Terrain gameobject here also holds `GradientNoiseTerrainGenerator.cs`.
- All values used in calculations can be accessed from the inspector.

## 3) Voronoi Diagrams/Worley noise

The main showcase of voronoi diagrams is found in the `VoronoiScene.unity`.<br>
The Terrain gameobject holds `VoronoiTerrainGenerator.cs` which creates the shown terrain.<br>
The `exampleSprite` (child object of `Main Camera`) holds `VoronoiDiagram.cs` which creates the initial voronoi diagram shown.
- All values used in calculations can be accessed from the inspector.

## 4) Blue noise

The main showcase of blue noise and layering it with gradient noise and voronoi is also found in `VoronoiScene.unity`.<br>
The Terrain gameobject holds `BlueNoiseSprite.cs` which creates the assets placed by the noise function.
- All values used in calculations can be accessed from the inspector.

## 4) Sources, references and helpful links

General:
- [https://www.ronja-tutorials.com/](https://www.ronja-tutorials.com/)

Gradient/Simplex/Perlin noise:
- [https://en.wikipedia.org/wiki/Gradient_noise](https://en.wikipedia.org/wiki/Gradient_noise)
- [https://en.wikipedia.org/wiki/Simplex_noise](https://en.wikipedia.org/wiki/Simplex_noise)
- [https://weber.itn.liu.se/~stegu/simplexnoise/simplexnoise.pdf](https://weber.itn.liu.se/~stegu/simplexnoise/simplexnoise.pdf)
- [https://de.wikipedia.org/wiki/Perlin-Noise](https://de.wikipedia.org/wiki/Perlin-Noise)
- [https://adrianb.io/2014/08/09/perlinnoise.html](https://adrianb.io/2014/08/09/perlinnoise.html)

Worley noise:
- [https://thebookofshaders.com/12/](https://thebookofshaders.com/12/)
- [https://en.wikipedia.org/wiki/Voronoi_diagram](https://en.wikipedia.org/wiki/Voronoi_diagram)
- [https://](https://github.com/jushii/WorleyNoise)[github.com](https://github.com/jushii/WorleyNoise)[/](https://github.com/jushii/WorleyNoise)[jushii](https://github.com/jushii/WorleyNoise)[/](https://github.com/jushii/WorleyNoise)[WorleyNoise](https://github.com/jushii/WorleyNoise)

Blue noise:
- [https://de.wikipedia.org/wiki/Hard-core-Prozess](https://de.wikipedia.org/wiki/Hard-core-Prozess)
