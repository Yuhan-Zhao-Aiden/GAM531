# GAM531 Midterm Project (Monolith)

## Game Title

- Game name: Monolith
- Description: A 3D scene build using OpenTK with basic physics engine
<p align="center">
  <img src="Screenshot 2025-10-30 223326.png" width="1000" alt="Centered cube render">
</p>

## Gameplay Instructions

- W/A/S/D: Move the character forward, left, back and right.
- Mouse: Look around
- Space: Jump
- Left Mouse Button: fire bullet
- ESC: Unlock mouse, click the window to grab mouse again

## Features
- Multiple objects in the 3D scene (ground + crate + Monolith + projectiles)
- Camera movement controlled by mouse
- Phong lighting
- 4 textures
- Physics simulation & Interaction (Gravity + Rigid body)
  - Jumping
  - Shooting projectiles
- Code organized in OOP style

## How to Run
``` bash
# clone the repo
git clone https://github.com/Yuhan-Zhao-Aiden/GAM531.git
cd GAM531/YuhanZhao_Midterm_Game

# Run the project
dotnet run --project Game
```

- Or open the project in Visual Studio (Game.sln) and click Run.

## Additional Implementation Details

- Each Object in the scene is contained inside SceneObject class with it's
  - Mesh
  - Texture
  - Transformation
  - Physics
- Collision is detected and resolved using AABB (Axis-alined Bounding Box)
- Projectiles have a max count of 10.
- Game window is set to run at 120fps with VSync on

## Credits

### Textures

- https://www.poliigon.com/texture/heavily-corroded-metal-texture/3219
- https://www.poliigon.com/texture/ledger-stone-wall-texture-mixed-gray/7000
- https://ca.pinterest.com/pin/858920960155361015/
- [Projectile Texture](https://www.freepik.com/free-photo/top-view-dark-background-texture-concrete-surface_11776758.htm#fromView=keyword&page=1&position=25&uuid=f5e5988f-eeb0-4a58-913c-bef5a7608f95&query=Rough+metal+texture)

### Guide

- https://www.youtube.com/watch?v=-_IspRG548E
- https://www.youtube.com/watch?v=oOEnWQZIePs&t=91s