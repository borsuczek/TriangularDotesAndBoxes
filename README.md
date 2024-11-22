# Dots and Boxes in a Triangular Version

## Description
A desktop version of the classic 'Dots and Boxes' game, but played using triangles. It supports gameplay between two human players, a human player against a computer opponent, and two computer opponents with varying levels of sophistication. One computer opponent follows a less optimal strategy, while the other uses recursive algorithms to aim for victory.

## Installation

To run the application, follow these steps:

1. Clone this repository to your local machine:
   ```
   git clone https://github.com/borsuczek/TriangularDotesAndBoxes.git
   ```
2. Navigate to the Exe folder
3. Double-click on Dotes.exe to launch the application

## Game Rules
After choosing the gameplay mode and board size the game begins.

### 1. Two-Player Game
- The current player can click any available dot. Clicking a dot highlights it and blocks not available dots. The player can either unclick or select another allowed dot to create a line.
- If a triangle is formed after placing a line, the current player gets another turn and gets a point. Otherwise, the turn passes to the next player.

### 2. Playing Against the Computer
- The rules for playing against the computer are the same as for two human players.
- The computer player has a difficulty level (set during game creation) which influences its strategy.
- After the human player's turn, the computer automatically makes its move.

### 3. Computer vs. Computer
- The game between two computer players is similar to the human vs. human mode, except the moves are made consecutively without visual indicators for impossible moves.
- This mode is mainly for testing and comparing strategies between different AI levels.

### 4. Computer Strategy
The computer players use algorithms to determine valid moves. The strategy varies based on the set difficulty:

- **Easy (Random)**: The computer randomly selects available dots and connects them without any strategic consideration.
- **Hard (Strategic)**: The computer evaluates potential moves based on whether they can form a triangle or if it would allow the opponent to do so. It uses a profit matrix to calculate the value of each move, prioritizing those that complete a triangle. The computer recursively analyzes possible future moves to minimize the opponent's gains.

## Credits
This game was created by Aleksandra Borsuk and Kajetan Siebieszuk.
