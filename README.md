# PIVEksamenAnton
Eksamens aflevering PIV Anton
## Overview of the Game:
The game is inspired by “A short hike” which lets the player move around freely and explore an island with different movements features, such as walking, running, climbing, jumping and flying/floating. For my version, the player can walk around and jump, with additional jumps being able to be performed up to a set number acquired. Additionally, with the same space button the user can hold it down and perform a gliding/flying movement, which lets the player float/reduce their gravity so they can glide around. The player can also collet golden feathers to increase their maximum amount of jumps available, as well as a set goal to touch, which reminiscent a bit of a short hike where the goal is getting to the top of a mountain. The goal for my game is to collect enough golden feathers, and utilizing the movement system in order to get to the top of a tree and claim victory by hitting the goal flag. The genre is exploration and collection, with platform mechanics.
Main parts of the game:
•	Player – A bird, moved around with WASD and space key
•	Golden feathers – Placed at specific points in the map, which gives additional jumps
•	Jumps – Able to perform multiple jumps in a row
•	Floating – Gliding which lets you lose altitude slower
Game features:
•	UI which shows amount of feathers/jumps available
•	A camera which converts simple 3D models to pixel
•	Particle effects for interactable objects
## Running It:
1. Download Unity >= 2022.3.11f1
2. Clone or Download the project 
3. The game requires a computer with a keyboard

## Scripts

### Scripts
o	ClaireMovement – Used for controlling the player, as well as the different state that are available. It also updates information for the UI script
o	FeatherJump – Used to check for collision and tag, and to call a function to increase jumps
o	GoalFlag – Checks also for collision and activate an assigned particle when touched
o	UIMangager – Used to handle the UI and call variable for the current and max jumps to change images, and to activate images on the UI
o	PixelatedCharacter – Used to output texture to a render texture, which another camera takes and scales up which creates a pixel effect

### Models & Prefabs

o	Low polly assets used to create the world: (https://assetstore.unity.com/packages/3d/environments/landscapes/low-poly-simple-nature-pack-162153)
o	Clair/player, feather and goal flag made with unity primitives and materials

### Animations:

o	Animations made for Clair in Unity, for the different states, idle, walking, jumping, floating

### Particles:

o	For the hail particles in the project: (https://assetstore.unity.com/packages/vfx/particles/environment/hail-particles-pack-62038)
o	Particles for the feather, and the confetti for the goal flag: (https://assetstore.unity.com/packages/vfx/particles/hyper-casual-fx-200333)

### Materials:
o	A skybox used for the world and immersion: (https://assetstore.unity.com/packages/vfx/shaders/free-skybox-extended-shader-107400)

| **Task**                                                                | **Time it Took (in hours)** |
|--------------------------------------------------------------------------------|------------------------------------|
|     Setting up Unity, making a project in GitHub                             |     1.5                            |
|     Research and   conceptualization of game idea                              |     1                              |
|     Searching for 3D models to add to the project                                         |     1                            |
|     Building 3D models from scratch – Clair, feather, goal flag                         |     1.5                              |
|     Making animations and setting it up                     |     2.5                              |
|     Player   movement                                                          |     6                            |
|     Combining player movement with camera(cinemachine) orientation, bugfixing             |     1.5                            |
|     Setting up camera with the pixel effect         |     1.5                              |
|     Understanding and implementing the particle system           |     2                              |
|     Creating the world          |     1.5                            |
|     Making UI elements                        |     1.5                            |
|     Collisions and bugfixing error with multiple collision all at once       |     2                            |
|     Playtesting   and bugfixing fringe cases in rigidbody incorrect physics    |     5                            |
|     Code   documentation                                                       |     1                              |
|     Making readme                                                              |     0.5                            |
|     **All**                                                                        |     **29.5**                           |

## References
-[How to make a  pixelated camera in unity:] (https://www.youtube.com/watch?v=L-tNbbov6Bo&t=355s)
-[Setting up cinemachine/getting ideas of how it works:] (https://www.youtube.com/watch?v=jPU2ri4ZwxM)
-[The developer of “A short hike” GDC talk:] (https://www.youtube.com/watch?v=ZW8gWgpptI8&t=355s) 
-[How to make a character walk in Unity:] (https://www.youtube.com/watch?v=hlO0XlqZFBo&t=493s) 
