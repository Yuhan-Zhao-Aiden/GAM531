# GAM531 A9 - 3D Collision Detection

This is a 3D game with AABB collision detection, step on enemies to defeat them.

<p align="center">
  <img src="Screenshot 2025-11-21 225241.png" width="1000" alt="Centered cube render">
</p>

## Collision detection
- AABB collision detection
  - This game uses AABB, most efficient collision detection with collision borders align with axis. 
  - Each collider is represented by a box with Min and Max.
  - Collision can be easily detected using the AABB.Intersects() function, which checks for intersection on all 3 axis, if all of them overlap, there's a collision
  - Each GameObject can have BoxCollider that defines the size of the AABB.
  - The PhysicsSystem class handles all collision check in a nested loop (Check all pairs of BoxCollider)

## Collision and Movement
- Jumping
  - Gravity pulls player object down
  - PhysicsSystem detects collision between player and ground
  - Collision is resolved by PhysicsSystem, so that player lands on the ground (pushed upward)
  - Then _isGrounded is set to true so the player can jump again
- Destroy Enemy
  - PhysicsSystem detects player and enemy bounding boxes overlapping
  - Then OnCollision method is called on both player and enemy
  - Check player is above enemy, also check if the player is falling
  - Yes => Enemy.Destroy() is called + bounce player up
  - No => Enemy kills player and restart 

## How to Run
```bash
git clone https://github.com/Yuhan-Zhao-Aiden/GAM531.git
cd GAM531/A9
dotnet run
```
Or open A9.sln in Visual Studio and click run.

## How to Play
- W/A/S/D: Move
- Space: Jump
- Mouse: Look around

## Features
- Jump on enemies to Stomp them and score
- Third person camera
- Phong lighting
- Physics (AABB BoxCollider)
- 5 waves of enemies, +1 enemy per each wave
- Random enemy spawn
- Score system and game state

## Credits
- https://www.poliigon.com/texture/ledger-stone-wall-texture-mixed-gray/7000