# Light
![GitHub](https://img.shields.io/github/license/Floneyyang/light)
![Website](https://img.shields.io/website?url=https%3A%2F%2Ffloney.itch.io%2Flight)
![GitHub code size in bytes](https://img.shields.io/github/languages/code-size/Floneyyang/Light)
### Overview
![](/light1.gif)
Light is a 2.5D puzzle-platformer maze game that controls a blue light bulb to find the key in a dark maze while avoiding enemies.

### Design:
- This game focuses on the idea of light and is playing around with the concept of light under different situations to create interesting enemy/player interaction and puzzles. 
- The player has two modes: 1) lights-on: The player can move at full speed but can be detected by the enemy’s vision; 2) lights-off: The player will be invisible to the enemy but moves at a very slow speed. 
- The player’s health is indicated through the level of light it emits. 


### Engineering:
- Inheritance/Polymorphism based classes for input handler and command with decoupling design principle
- Mesh Generation using ray casting with 3D math-based edge detection. This mesh generation technique is based on @Sebastian Lague’s tutorial on the [field of view visualisation](https://github.com/SebLague/Field-of-View). 
- Ray casting enemy detection system
- Enemy pathfinding: I used the [NavMeshAgent](https://docs.unity3d.com/ScriptReference/AI.NavMeshAgent.html) AI system for enemy auto pathfinding.
- Enemy state logic: enemies have three states: 1) patrol 2) alert 3) attack

### Links: 
<img alt="Itch.io" src="https://img.shields.io/badge/Itch%20-%23FF0B34.svg?&style=for-the-badge&logo=Itch.io&logoColor=white"/> [Web Build](https://floney.itch.io/light)

![](/light2.gif)
