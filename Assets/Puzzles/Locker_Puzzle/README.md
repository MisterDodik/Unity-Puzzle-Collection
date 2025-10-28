# **Locker Puzzle**

**Type**: Path Puzzle

**Goal**: Guide the ball from the starting triangle to the target triangle


### 🎮 Gameplay Description

- The puzzle consists of a **grid of triangles**, each with a **path** drawn on it.

- A ball must travel from the **starting triangle** to the **target triangle**.

- **Movement rules**:

  - The ball moves to a **neighboring** triangle when it is **clicked**, but only if the triangle is **interactable**.

  - A triangle is **interactable** if its path **connects directly** to the current triangle where the ball is located.

- Rotational challenge:

  - When the ball moves to a triangle, the triangle **rotates 120°**, rotating its path as well.

  - This can **alter future moves**, so players have to plan ahead.


### 🛠 Implementation Notes

- Triangle interaction is implemented using **click detection**.

- Rotation logic updates the triangle path and **affects connectivity**.


### 🎬 Demo / GIF

![Locker Puzzle Demo](./demo.gif)


### 💡 Notes / Highlights

- The puzzle includes **Reset** and **Skip** buttons, allowing players to restart the puzzle or move past it if needed.