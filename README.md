# Pac-Man Style Game

This is a simple Pac-Man-style prototype game developed in Unity. The game features a player who navigates a procedurally generated maze, collects coins, and avoids enemies.

## How to Play

- **Movement:** Use the arrow keys or WASD to move the player through the maze.
- **Objective:** The objective is to collect all the coins in the maze without getting caught by the enemies.
- **Game Over:** The game ends when an enemy catches the player. The player will then respawn at a random location, and their score will be reset.

## Features

- **Procedural Maze Generation:** The mazes are generated using a recursive backtracking algorithm, which creates a unique maze every time the game is played.
- **Player Controls:** The player can be controlled using the arrow keys or WASD.
- **Enemy AI:** The enemies in the game move randomly through the maze.
- **Scoring System:** The player's score increases as they collect coins.

## Scripts

- **`MazeGenerator.cs`:** This script is responsible for generating the maze.
- **`PlayerControler.cs`:** This script controls the player's movement, score, and interactions with the game world.
- **`EnemyControler.cs`:** This script controls the enemy's movement and interactions with the game world.
- **`MazeRenderer.cs`:** This script is responsible for rendering the maze.

## How to Run

1.  **Clone the repository:**
2.  **Open the project in Unity:**
    - Open Unity Hub.
    - Click on the "Add" button.
    - Select the cloned repository's folder.
3.  **Run the game:**
    - Open the `MainScene` scene from the `Assets/Scenes` folder.
    - Click on the "Play" button.
