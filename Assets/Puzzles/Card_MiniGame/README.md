# **Card Minigame**

**Type**: Turn-based Card Minigame

**Goal**: Reduce the opponent’s HP to zero before your own


### 🎮 Gameplay Description

- At the start, the player **selects 3 cards out of 6** available.

- Gameplay proceeds in **turns**:

  - The player selects a card on the field and a target enemy card.

  - The player’s card performs an **attack animation** and deals damage based on its stats.

  - The opponent then performs the same process.

- If a card’s HP drops to 0, it is removed from the field, and any damage overflow affects the player.

- Players can **play cards from their deck onto the field** during their turn.

  - Some cards have **special abilities**:

    - Heal the player or opponent depending on who played the card.

- The game ends when **one player’s HP reaches zero**.

- A small **tutorial level** is included at the beginning to explain all mechanics.


### 🛠 Implementation Notes

- Turn-based logic implemented with scripts managing card selection, attacks, and special effects.

- Each card has its **stats, abilities, and state** tracked.

### 🎬 Demo / GIF

![Card Minigame Demo](./demo.gif)


### 💡 Notes / Highlights

- Shows **turn-based** gameplay design.

- The puzzle includes **Reset** and **Skip** buttons, allowing players to restart the puzzle or move past it if needed.