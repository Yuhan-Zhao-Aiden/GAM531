## Basic Physics & Animation Upgrade

1. **Entity foundation**
   - Expand `SceneObject` (or add a `Player` struct) with position, velocity, size, and reference to `SpriteRenderer`.
   - Let `Game` own the player instance plus a list of static ground tiles.

2. **Asset loading**
   - Load `Animation/_Idle.png`, `Animation/_Jump.png`, `Animation/_Fall.png`, and `Assets/ground.png`.
   - Register the jump/fall clips (3 frames each, same dimensions as idle) with `SpriteRenderer`.

3. ~~**Physics constants/state**~~ ✅
   - ~~Add gravity, jump impulse, and damping values near the top of `Game`.~~
   - ~~Track `IsGrounded` and per-frame delta clamping to keep integration stable.~~

4. ~~**Input & motion**~~ ✅
   - ~~In `OnUpdateFrame`, check keyboard input; when grounded and jump key pressed, apply vertical impulse.~~
   - ~~Integrate velocity & position each frame (`vel += gravity * dt`, `pos += vel * dt`), keeping coordinates in pixel space.~~

5. ~~**Animation switching**~~ ✅
   - ~~Add a helper to pick the active clip: idle when grounded & near-zero velocity, jump when `vel.Y > 0`, fall when `vel.Y < 0`.~~
   - ~~Restart clips when state changes to keep transitions crisp.~~

6. ~~**Ground tiles & collisions**~~ ✅
   - ~~Create a `GroundTile` helper that loads `ground.png`, tiles it along the window bottom, and exposes AABB bounds.~~
   - ~~During physics update, detect overlap between the player feet and tile tops; snap Y position, zero `vel.Y`, and set `IsGrounded`.~~

7. **Rendering adjustments**
   - Replace the single `_model` transform with per-entity matrices (player + ground tiles) before drawing.
   - Ensure the player sprite is bottom-aligned to match collision logic.

8. **Resize handling**
   - When the framebuffer changes, rebuild the projection and reposition the ground tile row to the new window bottom so collisions stay in sync.
