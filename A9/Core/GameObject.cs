using A9.Components;

namespace A9.Core;

public class GameObject
{
  public Transform Transform { get; private set; }
  public bool IsActive { get; set; } = true;

  private List<IComponent> _components = new List<IComponent>();

  public GameObject()
  {
    Transform = new Transform();
  }

  public GameObject(Transform transform)
  {
    Transform = transform;
  }

  // Component
  public void AddComponent(IComponent component)
  {
    component.GameObject = this;
    _components.Add(component);
  }

  public T? GetComponent<T>() where T : class, IComponent
  {
    return _components.OfType<T>().FirstOrDefault(); // Filter
  }

  public IEnumerable<T> GetComponents<T>() where T : class, IComponent
  {
    return _components.OfType<T>();
  }

  // Virtual methods for game object behavior
  public virtual void Update(float deltaTime)
  {
    if (!IsActive) return;

    foreach (var component in _components)
    {
      component.Update(deltaTime);
    }
  }

  public virtual void Render()
  {
    if (!IsActive) return;

    foreach (var component in _components)
    {
      if (component is MeshRenderer renderer)
      {
        renderer.Render();
      }
    }
  }

  public virtual void OnCollision(GameObject other)
  {
  }
}