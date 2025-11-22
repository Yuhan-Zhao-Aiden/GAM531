namespace A9.Components;

using A9.Core;

public interface IComponent
{
  GameObject? GameObject { get; set; }
  void Update(float deltaTime);
}
