namespace PhongLighting
{
  public class Utility
  {
    public static (float[] vertices, uint[] indices) BuildCube()
    {
      float[] vertices =
      {
// FRONT (+Z, normal 0,0,1)
        -0.5f, -0.5f, +0.5f,   0f, 0f, 1f,
         0.5f, -0.5f, +0.5f,   0f, 0f, 1f,
         0.5f,  0.5f, +0.5f,   0f, 0f, 1f,
        -0.5f,  0.5f, +0.5f,   0f, 0f, 1f,

        // BACK (-Z, normal 0,0,-1)
        -0.5f, -0.5f, -0.5f,   0f, 0f,-1f,
        -0.5f,  0.5f, -0.5f,   0f, 0f,-1f,
         0.5f,  0.5f, -0.5f,   0f, 0f,-1f,
         0.5f, -0.5f, -0.5f,   0f, 0f,-1f,

        // LEFT (-X, normal -1,0,0)
        -0.5f, -0.5f, -0.5f,  -1f, 0f, 0f,
        -0.5f, -0.5f,  0.5f,  -1f, 0f, 0f,
        -0.5f,  0.5f,  0.5f,  -1f, 0f, 0f,
        -0.5f,  0.5f, -0.5f,  -1f, 0f, 0f,

        // RIGHT (+X, normal +1,0,0)
         0.5f, -0.5f, -0.5f,   1f, 0f, 0f,
         0.5f,  0.5f, -0.5f,   1f, 0f, 0f,
         0.5f,  0.5f,  0.5f,   1f, 0f, 0f,
         0.5f, -0.5f,  0.5f,   1f, 0f, 0f,

        // TOP (+Y, normal 0,+1,0)
        -0.5f,  0.5f, -0.5f,   0f, 1f, 0f,
        -0.5f,  0.5f,  0.5f,   0f, 1f, 0f,
         0.5f,  0.5f,  0.5f,   0f, 1f, 0f,
         0.5f,  0.5f, -0.5f,   0f, 1f, 0f,

        // BOTTOM (-Y, normal 0,-1,0)
        -0.5f, -0.5f, -0.5f,   0f,-1f, 0f,
         0.5f, -0.5f, -0.5f,   0f,-1f, 0f,
         0.5f, -0.5f,  0.5f,   0f,-1f, 0f,
        -0.5f, -0.5f,  0.5f,   0f,-1f, 0f,
      };
      
      uint[] indices =
      {
        // front
        0,1,2,  2,3,0,
        // back
        4,5,6,  6,7,4,
        // left
        8,9,10, 10,11,8,
        // right
        12,13,14, 14,15,12,
        // top
        16,17,18, 18,19,16,
        // bottom
        20,21,22, 22,23,20
      };
      return (vertices, indices);
    }


  }
}