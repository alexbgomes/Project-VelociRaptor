# Project-VelociRaptor
CPS643 Project

---

## 1 - What is the goal of your project?
The goal of this project is to create a bullet hell VR game in space which is inspired by the iconic old school game [Raptor: Call of The Shadows](https://store.steampowered.com/app/336060/Raptor_Call_of_The_Shadows__2015_Edition/). 

---
## 2 - What type of project are you proposing?
We are proposing a video game style project with very general flight simulation mechanics–a flight stick to move the plane, buttons to fire guns, etc. There will be other interactables such as buttons and levers inside the cockpit of the spaceship that allow the user to use different power ups and abilities. The spaceship will move freely up, down, left and right. However, there will be a world boundary which keeps the ship in a contained area.

---
## 3 - Why will a fully immersive virtual environment be necessary or be helpful for your project? 
The main idea of the project is to have two views the player can toggle between. The first view will be inside the cockpit of the spaceship. This will allow the user to interact with different objects and be immersed into the experience when dodging projectiles from enemies. Usually with bullet hell games it's mainly top-down which allows the user to anticipate the patterns easily and dodge accordingly. With incorporating a fully immersive virtual environment, the player can feel the intensity of the game more. We have also been thinking about adding a third person view, but this is still up for discussion.

---
## 4 - What advantages unique to VR will your project focus on? • You may choose 1 or 2 categories
The two categories we are aiming to specialize in with this project are Simulation and Interaction. Instead of having many controls, we want the player to understand how to play straight out of the gate, just using their instincts and natural reactions. When a projectile is coming towards them, their reaction should be naturally to move out of the way using the flight stick. We will be using the grip button to hold the flight stick and the trigger button to fire. Any interactable menu will be built into the cockpit and can be interacted with pushing your finger down on the prompts. We are hoping to add different levers and buttons around the cockpit that invoke different abilities and power ups the player picks up from killing enemies. 

---
## 5 - What will the surroundings of your VE look like to your user?
We are thinking of making the surrounding area look like space, with Earth being under the player. The main idea of the game is that aliens are invading Earth and you, the player, fly out to space to fight them before they enter our atmosphere. 

---
## 6 - Is the space navigable or is the user constrained to a specific position or small space?
The user is constrained to the cockpit of the spaceship. However, the ship is free to move in the confined space so that the player cannot move completely out of the range of the enemies. 

---
## 7 - Do you plan to use a Non-Euclidean Scene?
No, the scene will be grounded in sci-fi reality, on a euclidean space.

---
## 8 - What input device will your project utilize?
Standard VR controllers and HMD will be used, both of us will specifically use the Oculus Quest 2 controllers.

---
## 9 - Will your project make use of haptics (i.e. force feedback)
There will be considerations made for vibration feedback from certain user inputs such as firing weapons.

---
## 10 - Will your project make use of Spatialized Sound?
There will be considerations made to have the sound source of certain effects such as explosions sound effects occurring at the point of destruction such that the sound will be loaded and then diminish by inverse-square law. We can also incorporate error noises that occur from one side of the cockpit when the player takes too much damage.

---
## 11 - What basic interactions will your application implement? (Be as detailed as you can) 
Direct Object Manipulation: The primary mode of movement for the user will be done using the flight stick which will act as a middle ground between the user’s controller and the ship’s movement.
Indirect Object Manipulation: Some menu items on the cockpit will require these button/sider objects to be manipulated by the user.
Scripted Object Interaction: There will be switches and levers in the cockpit that the user may interact with, these objects will be triggered via a script to shift between states.

---
## 12 - How are you preventing the user from getting sick while they are immersed in your game/application
Implement a multi-part helmet system where they may choose to have a point in the center which the user may focus on in order to reduce motion sickness or blinders to reduce the field of vision.

---
## 13 - How will the user navigate the environment?
The user will navigate the environment via the spaceship vehicle. The vehicle will always be moving forward, traversing the scene. The user will have control over moving the vehicle on a 2 dimensional plane, shifting left, right, and up, down.

---
## 14 - How will the user perform system control within the virtual environment?
The game will implement a mixed interface consisting of a graphic menu incorporated into the main cockpit screen and several diagetic levers and buttons around the cockpit. There will be no floating menus. Everything should feel natural to the user. 

---
## 15 - What type of simulation/animation (if any) is required?
There will be different types of bullets as well as enemies in their own vehicles. These will all require animations. The spaceships will also need an explosion animation when they have no more life points. We will be using kinematics mainly and rigid body physics possibly. 

---
## 16 - Which advanced topic does your project incorporate?
The project may incorporate haptic feedback via the controller’s vibration mechanism. We may also utilize the particle system to create specific effects. It is also possible to look into gesture recognition such as placing your hand outside the view near your temple and pressing the trigger to activate some helmet mechanic. We can also look into procedural model generation with one of the assets that allows us to build customized ships.

---
## 17 - If working with a partner, what is the role of each team member? 
Both of us will be on the same page when working on the project with a high level of trust. This includes programming and creative design. If there is something one person doesn’t understand, the other will describe their thought process and ideas as best as possible through calls and drawing out visuals on a whiteboard.

---
## 18 - What coding will be necessary (be as detailed as possible – think carefully about what scripts you might need)?
There needs to be many levels of coding necessary for this project. There needs to be an effective way  to spawn enemies and specific projectile patterns in order for the player to avoid. This will all be kinematic and require simple collider detection with the player. The player may shoot out projectiles of varying physical properties which will also have collider detection. These player projectiles need to be done in a pooling pattern so that there is a limitation on how many of these objects are created and so that the occluded projectiles may be freed from memory. There will need to be a way to disallow the player from moving out of the world boundary. There will also be world-space menus that are intractable from the cockpit. These menus may require a pointer from the player’s hand that will use raycasting to select items or require physically collision with finger tracking. There will be item pickups from enemies that will enhance or modify the player’s performance accordingly. These need to be modular and organized in a extensible design pattern.

---
## 19 - Where will project assets come from (e.g. model meshes etc) and list some assets
The majority of project assets will likely come from the [Unity Asset Store](https://assetstore.unity.com/). A few assets that have been considered are:
- [Star Sparrow Modular Spaceship](https://assetstore.unity.com/packages/3d/vehicles/space/star-sparrow-modular-spaceship-73167)
- [Skybox Series](https://assetstore.unity.com/packages/2d/textures-materials/sky/skybox-series-free-103633#content)
- [Federation Corvette F3](https://assetstore.unity.com/packages/3d/vehicles/space/federation-corvette-f3-79860)
- [Alien Ships Pack](https://assetstore.unity.com/packages/3d/vehicles/space/alien-ships-pack-131137)
- [Quick Outline](https://assetstore.unity.com/packages/tools/particles-effects/quick-outline-115488)
- [War FX](https://assetstore.unity.com/packages/vfx/particles/war-fx-5669)
- [Beautiful Progress Bar](https://assetstore.unity.com/packages/2d/gui/icons/beautiful-progress-bar-free-194904#content)
- [sFuture Targeting](https://assetstore.unity.com/packages/3d/props/sfuture-targeting-83113#description)
- [Rockets, Missiles, & Bombs](https://assetstore.unity.com/packages/3d/props/weapons/rockets-missiles-bombs-cartoon-low-poly-pack-73141)

Some texture assets may be found using outside sources such as ambientCG. All of these assets are free to use and at the minimum, free for non-commercial use with accreditation.

---
## 20 - List any external projects/code that may be used and describe how you will incorporate, modify and extend this code
We may look at a few tutorials on [Unity’s official website](https://learn.unity.com/). No code will be copied but, we may incorporate general concepts that fit our needs. [Here](https://learn.unity.com/tutorial/creating-objects#), is an example.

---
## 21 - Include a rough schedule of the work
Monday, Wednesday, and Friday will be our official available days to work on this project in a collaborative environment (e.g. virtual meeting) and may decide to make adjustments/debugging sessions in between when time permits. 
