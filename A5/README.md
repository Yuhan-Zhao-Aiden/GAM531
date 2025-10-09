# GAM531 - A5 (Phong Lighting)

In this assignment, We implement Phong Lighting and camera movement in OpenTK

## How to run
- Requirement
  - .NET 8.0
  - OpenTK 4.*

- Run

  ```bash
  git clone https://github.com/Yuhan-Zhao-Aiden/GAM531.git
  cd GAM531/A5

  dotnet run
  ```

## Tools and Framework
- OpenTK

## Features
- Each vertex has normal that tells the direction where surface is facing, This is for calculating diffused lighting
- The Phong Lighting is composed by 3 components
  - Ambient: simulates light bounse in the room
  - diffused: simulates amount of light received by the surface
  - specular: simulates the reflection of the light source

- Camera movement
  - Press WASD to move the position of the camera
  - Press QE to increase and decrease yaw
  - No pitch control in this version