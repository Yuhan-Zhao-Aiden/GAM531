namespace ApplyTexture
{
  public class Utility
  {
    public static (float[] vertices, uint[] indices) BuildCuboid(float length, float side)
    {
      // Compute the absolute value of coordinates around origin
      float L = length * 0.5f; 
      float H = side * 0.5f;
      float W = side * 0.5f;

      // position + UV coordinates
      float[] vertices = new float[]
      {
          -L, -H,  W,   0f, 0f, // Note: Since we need to map UV, we need 24 vertices
          L, -H,  W,   1f, 0f, // instead of 8, each vertex might have different UV coordinates
          L,  H,  W,   1f, 1f,
          -L,  H,  W,   0f, 1f,

          -L, -H, -W,   1f, 0f,
          L, -H, -W,   0f, 0f,
          L,  H, -W,   0f, 1f,
          -L,  H, -W,   1f, 1f,

          -L, -H, -W,   0f, 0f,
          -L, -H,  W,   1f, 0f,
          -L,  H,  W,   1f, 1f,
          -L,  H, -W,   0f, 1f,

          L, -H, -W,   1f, 0f,
          L, -H,  W,   0f, 0f,
          L,  H,  W,   0f, 1f,
          L,  H, -W,   1f, 1f,

          -L,  H,  W,   0f, 0f,
          L,  H,  W,   1f, 0f,
          L,  H, -W,   1f, 1f,
          -L,  H, -W,   0f, 1f,

          -L, -H,  W,   0f, 1f,
          L, -H,  W,   1f, 1f,
          L, -H, -W,   1f, 0f,
          -L, -H, -W,   0f, 0f,
      };

      // 6 faces × 2 triangles × 3 indices = 36 indices
      uint[] indices = new uint[]
      {
          0, 1, 2, 2, 3, 0,
          4, 5, 6, 6, 7, 4, 
          8, 9,10,10,11, 8, 
        12,13,14,14,15,12, 
        16,17,18,18,19,16, 
        20,21,22,22,23,20 
      };

      return (vertices, indices);
    }
  }
}