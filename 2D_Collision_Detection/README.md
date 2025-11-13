# 2D Collision Detection

This is a simple demonstration of circle - AABB collision detection
ball bouncing between 2 blocks, on collision, the block involved will toggle visibility

SInce I don't have enough time, this is a much simpler version of breakout

## Analysis
- The ball size and brick dimension will not affect collision accuracy for a circle-AABB collision, since collision boarder of the circle is a circle, and it fits perfectly. However, if the circle use an AABB, bigger ball size means it will have more empty space around the 4 corners.
- false collision detection will happen if we use AABB for the ball, and the ball collide the brick with its corner
- For the simple version of the breakout, it doesn't matter if we use AABB-AABB or circle-AABB methods, since the ball only move in one dimension along x axis, there's no chance for the ball to collide with the brick at an angle.

==Can you please consider extending the deadline for this assignment, I'll try to finish the Breakout game if I can get more time, Thank you!==

* You can find the report in this repository or on blackboard

## How to run
```bash
git clone https://github.com/Yuhan-Zhao-Aiden/GAM531.git
cd GAM531/2D_Collision_Detection/2DCollision
dotnet run
```