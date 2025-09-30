# Cube Render

In This assignment we create a 3d cube that rotates, using OpenTK

## Libraries

- OpenTK 4.*
- OpenTK.Mathematics for creating rotation matrix

## How the cube render
- 8 vertices inside the vertices array, each with x,y,z coordinates and r,g,b color
- indices array define the triangles, 2 per each face of cube
- 2 GLSL scripts
  - Vertex shader: compute model * view * projection for each vertex
  - Fragment Shader: output color
- `OnUpdateFrame()`: defines the rotation of the cube, apply `Matrix4.CreateRotationY(angle)` to rotate 10 degree per frame.


## How to Run
1. Clone the repo
```bash
git clone https://github.com/Yuhan-Zhao-Aiden/GAM531_A3.git
cd GAM531_A3
```
2. Run tests
```bash
dotnet run 
```