Student Name: Andrew Knopf (andrew.knopf@digipen.edu)

Project Name: Sgt Eye in the Sky

What I implemented:
I implemented a row of soldiers marching forwards, turning left, then marching backwards while they make sounds.
I also implemented a sergeant who marches up and down the line of other marching soldiers and shouts commands at them.
Additionally, I implemented a line of men that run around the perimeter of the map, shouting when they reach a corner.
Lastly I attempted to implement boids, and the framework for all behaviors are there but it's either badly tuned, or there are bugs.

Directions (if needed): No directions needed to run the game

What I liked about the project and framework:
1. Project was fun because it was good experience working in proprietary DirectX engine
  - Even though it is an outdated distribution of DirectX
2. It was fun to implement boids
3. Agent framework is very simple to use

What I disliked about the project and framework: The framework lacks two major features that limit the ability of a student to get inspired
1. Lack of framework to destroy agents at runtime
	- This is very necessary for if a student wanted to make a particle system
	- Also necessary for more simulation type environments (have a basic model 'killing' another one)
2. Lack of animation or FBX loader support
- While a lot to implement, would be very encouraging for students to create more complex behaviors when they can see the result visually.

Any difficulties I experienced while doing the project:
1. Waited too long to start the project, essentially picking it up the last day because I was swamped with other classes-
   and (correctly) assumed that the project would be easy to implement with the given framework. But I shouldn't do it again
2. DirectX handedness was new to me and took a while to wrap my head around it

Hours spent: 6-8 hours (not sure I didn't really keep track)

New selector node (name): No selector nodes implemented

New decorator nodes (names):
 D_AtMapCorner
 D_RepeatForRandomTime
 D_RepeatTwoTimes
 D_RepeatUntilSuccess

10 total nodes (names):
 1. L_Fly
 2. L_March
 3. L_PlayCommandMarchSound
 4. L_PlayCommandPushUpSound
 5. L_PlayMarchSound
 6. L_StandUp
 7. L_TurnAround
 8. L_TurnLeft
 9. D_AtMapCorner
10. D_RepeatForRandomTime
11. D_RepeatTwoTimes
12. D_RepeatUntilSuccess 

4 Behavior trees (names): 
 1. Bird 
 2. Sarge
 3. Soldier
 4. RunningMan

Extra credit: No extra credit other than maybe it's the sickest one in the class?