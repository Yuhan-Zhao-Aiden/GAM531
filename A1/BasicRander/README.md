# FirstRender OpenGL (C# + OpenTK)

## Library
OpenTK is used in this assignment for cross-platform

## How to Run

### Prerequisites
- [.NET 8 SDK](https://dotnet.microsoft.com/en-us/)

### Steps
```bash
# clone the project
git clone https://github.com/Yuhan-Zhao-Aiden/GAM531_A1.git
cd GAM531_A1

# run
dotnet run --project FirstEngine
```

## Extra Notes
- The rectangle is drawn by render 2 triangles with VBO, VAO and EBO
- EBO is a GPU buffer of index of vertices that tells GPU which vertices for each triangle