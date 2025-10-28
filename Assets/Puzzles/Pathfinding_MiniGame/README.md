# **Pathfinding Minigame**

**Type**: Drag-and-Drop / Avoidance & Spatial Puzzle

**Goal**: Solve two separate puzzles: catch all fireflies and untangle strings without intersections


### 🎮 Gameplay Description

1️⃣ **Fireflies Puzzle**

- The player **drags** a lantern across the screen to **catch** fireflies.

- Fireflies **move away** from the lantern while it is being dragged.

- If the player **releases** the lantern, all fireflies **escape**, and the puzzle must be retried.


2️⃣ **Strings Puzzle**

- The player must **arrange** multiple strings on the screen.

- Strings must be positioned so that they **do not intersect** anywhere.

- Includes **multiple** levels.



### 🛠 Implementation Notes

- **Fireflies Puzzle**:

  - Lantern and fireflies interact via drag-and-drop logic and **distance-based movement**.

  - Fireflies track the lantern position in real-time for **avoidance** behavior.

- **Strings Puzzle**:

  - Strings use **LineRenderers** with **custom** attributes.

  - Intersection detection ensures no two strings overlap.


### 🎬 Demo / GIF

![Pathfinding Minigame Demo](./demo.gif)
