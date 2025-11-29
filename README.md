# Die: The Rolling Dice Game

Welcome to Die! This game is a heavily improved remake of the original hit game [DIE](https://tnaz.itch.io/die) make for GMTK 2022. It offers enhanced features, a new 2d art style, and more!

## ✒️ Description

In this combat round marathon you will traverse 9 distinct levels in order to achieve ascension. Players will play as a newly freed die and will attempt to liberate its brethren while avoiding the Corrupted. The land of dice is a land of balance. The Corrupted keep the Controlled in check, preventing one from ascending and keeping the world from being destroyed. This game is the legend of Die, the rolling die who saved it all. Or well... tried to. In the many timelines of Die, there were times of failure. Times when Die lost all his dots and became one with those he despises. Will your time be different? Will you achieve ascension?

## 🕹️ Gameplay

Players begin in Level 1 with the ability to move Die around the lush Dietopia plains by using `ARROW KEYS`. The goal is for them to liberate all the Controlled. In simpler terms, they must collide with the Green Dice in order to reduce their dots to 0, "liberating" them from existence. Players must avoid the Corrupted, the Red Dice, while navigating through the level. Colliding with a Corrupted will strip a dot away, but also weaken the Corrupted itself, making it lose a dot, since you will absorb some of its Corrupted essence.

Level 2 is where Die makes his first step towards ascension and learns the ability to manipulate time around him. This allows the player to use the new dash ability by hitting the `Z` key, allowing for a burst of motion in a direction. Players will be able to test out this new dash ability in the much larger Dietopia pits, while chasing a larger selection of Controlled.

With Die becoming more unstable, he is now able to phase as of Level 3, the Dietopia rock deposits. Phasing allows the player to go "through" anything inside the bounds of the map by clicking the `X` key. Unfortunately, Die's unstable state causes him to lose his touch of phasing rather quickly, also requiring a period of time to recharge. Due to new developments, security has been increased and more Corrupted are patrolling.

Levels 4, 5, and 6 follow the story of Die as he progresses through the Dietopia desert, factory, and frosted wasteland. Featuring larger maps with different designs, more Corrupted, and more Controlled, drastically changing the difficulty of the game.

Finalizing the increase of difficulty is the Final 3. Levels 7, 8, 9, is where Die has achieved catastrophic levels of destruction. Level 7 Dustopia starts to tear between cold and hot. Following this level, in level 8 the player will complete the ending of the world, having the world crumble into magma. Finally Die will ascend and will exist in the void, where the player will conquer the nightmares of Die in one last fight. During these levels, the player will receive 1 extra dot at the start of each level due to Die nearing his ascension. The game will be hard, but not impossible.

## 📃 Requirements

1. The player can move Die using the `ARROW KEYS`.
2. The player can use a dash by clicking the `Z` key.
3. Dashing can only be done again after a cooldown.
4. The player can phase for a small period of time by clicking the `X` key.
5. Phasing can only be done again after a cooldown.
6. Dashing is unlocked at level 2 and phasing is unlocked at level 3.
7. The player can access settings by hitting the `ESC` key or by clicking on settings in the startup page before a new run.
8. Settings are persistent.
9. Can adjust sound in settings.
10. Can set game resolution in settings.
11. Can set controls in settings.
12. On levels 7, 8, and 9 the player will receive a new dot if not already at max.
13. Every level changes to the appropriate level layout and generates the proper enemies and targets.
14. The player can collide with the targets, reducing the life of the target.
15. Once a target's health reaches 0, it dies.
16. The player takes damage if it collides with an enemy.
17. Colliding with an enemy also reduces its life.
18. If an enemy reaches 0 life it dies.
19. The player must complete the run in 1 sitting.
20. Reaching 0 lives results in losing the game.
21. Completing level 9 results in winning the game.

### 🤖 State Diagram

#### Game State Diagram

![Game State Diagram](./Game/Content/Images/Diagrams/GamePlayStateDiagram.png)

#### Dice State Diagram

![Dice State Diagram](./Game/Content/Images/Diagrams/DiceStateDiagram.png)

#### Player Dice State Diagram

![Player Dice State Diagram](./Game/Content/Images/Diagrams/PlayerDiceStateDiagram.png)

### 🗺️ Class Diagram

#### Dice Class Diagram

![Dice Class Diagram](./Game/Content/Images/Diagrams/DiceClassDiagram.png)

### 🧵 Wireframes

#### Main Menu

![Main Menu](./Game/Content/Images/Wireframes/MainMenu.png)

- **Play**: will start the game.
- **Settings**: will allow the player to ajust their settings before the game starts.
- **Quit**: will exit the game nicely :D

#### Game Over (You lose)

![Game Over](./Game/Content/Images/Wireframes/GameOver.png)

- **Play**: will restart the game.
- **Quit**: will exit the game nicely :D

#### You Win

![You Win](./Game/Content/Images/Wireframes/YouWin.png)

- **Play**: will restart the game.
- **Quit**: will exit the game nicely :D

#### Gameboads

For the gameboards, they are all designed using Tiled off of my tilesheets for each level. Here they are below.

#### Level 1

![Level1](./Game/Content/Images/Levels/PNG/level1.png)

#### Level 2

![Level2](./Game/Content/Images/Levels/PNG/level2.png)

#### Level 3

![Level3](./Game/Content/Images/Levels/PNG/level3.png)

#### Level 4

![Level4](./Game/Content/Images/Levels/PNG/level4.png)

#### Level 5

![Level5](./Game/Content/Images/Levels/PNG/level5.png)

#### Level 6

![Level6](./Game/Content/Images/Levels/PNG/level6.png)

#### Level 7

![Level7](./Game/Content/Images/Levels/PNG/level7.png)

#### Level 8

![Level8](./Game/Content/Images/Levels/PNG/level8.png)

#### Level 9

![Level9](./Game/Content/Images/Levels/PNG/level9.png)

### 🎨 Assets

The goal is to make this game be my sole creation while respecting the atmosphere of 2d top down pixel art games. To achieve this, 95% of assets of any sort will be made by me. This being said, things like fonts I will be using ones from online that are copyright free. Attributions will be made accordingly.

#### 🖼️ Images

All images are personally made. Below are the dice spritesheets I made for this game.

#### Die (You as the player)

![Die](./Game/Content/Images/player_dice.png)

#### Dash (When Die is dashing)

![Dash](./Game/Content/Images/player_dash_dice.png)

#### Ascension (When at level 9 / Die is phasing)

![Ascension](./Game/Content/Images/player_ascension_dice.png)

#### Target Dice (Controlled)

![Target](./Game/Content/Images/target_dice.png)

#### Enemy Dice (Corrupted)

![Enemy](./Game/Content/Images/enemy_dice.png)

#### ✏️ Fonts

For fonts, I'm using a copyright free font (has a liscence permitting the use of this font for any means, in any capacity) found on itch.io.

[PeaberryBase](https://emhuo.itch.io/peaberry-pixel-font)

#### 🔊 Sounds

I've not decided if I will be making all the music or not. Sound Effects will be made by me. This section will be updated appropriately.
