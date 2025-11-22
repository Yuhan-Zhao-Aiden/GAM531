using A9.Core;
using A9.Graphics;

namespace A9.Components;

public class MeshRenderer : IComponent
{
  public GameObject? GameObject { get; set; }
  public Mesh? Mesh { get; set; }
  public Material? Material { get; set; }

  public MeshRenderer(Mesh mesh, Material material)
  {
    Mesh = mesh;
    Material = material;
  }

  public void Update(float deltaTime)
  {
  }

  public void Render()
  {
    if (GameObject == null || Mesh == null || Material == null) return;

    Material.Apply(GameObject.Transform);

    Mesh.Draw();
  }
}
