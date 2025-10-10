# GAM531 - A6 (Camera movement)

In this assignment I improved camera control in A5 by adding mouse movement

## How to run
- Clone the repo
```bash
git clone https://github.com/Yuhan-Zhao-Aiden/GAM531.git
```
- Run the project
```bash
cd A6
dotnet run
```

## Libraries and tools
- .NET 8.0
- OpenTK

## Features
- Upgrade camera movement from A5
  - Added pitch movement
  - Pitch and yaw can be update with mouse movement

- Refactor camera function into it's own module

- Camera movement
  - Keyboard WASD moves the position of camera (independent from framerate)
  - Mouse control yaw and pitch of camera
  - Mouse wheel control FOV

- Camera limitation
  - -90f <= pitch <= 90f
  - 30f <= FOV <= 90f