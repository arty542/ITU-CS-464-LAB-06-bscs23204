# CS464 Lab 06 — BSCS23204

Lecture 07: enemies, hazards, health, lives and power-ups.

Open the project in **Unity 6000.3.24f1**, open `Assets/Scenes/Level01.unity` and press Play. All five scenes are enabled in Build Profiles.

**Controls:** WASD or arrows to move, Space to jump, R to restart. The result panels also have restart buttons, and winning opens the next level.

## Levels

| Level | Objective | Challenge | Lives |
|---|---|---|---|
| 1 — Training Courtyard | Collect any 5 coins and reach the exit. | Safe collection, bounce balcony and gap jump. | 3 |
| 2 — Timed Escape | Reach the exit within 60 seconds. | Spikes, a narrow bridge and a sideways gap. Coins are bonuses. | 3 |
| 3 — Coin Sweep | Collect all 10 coins, then reach the exit. | A patrol/chase/return guard and spike beds. | 2 |
| 4 — Survival Workshop | Survive 30 seconds inside the teal arena, then reach the exit. | Crusher, sliding spikes, arena spikes and a faster guard. Coins are bonuses. | 2 |
| 5 — Final Escape | Collect 10 coins and reach the exit within 90 seconds. | Mixed hazards, the fastest guard, two speed potions and a final gap. | 2 |

Screenshots: [Level 1](Docs/level01.png) · [Level 2](Docs/level02.png) · [Level 3](Docs/level03.png) · [Level 4](Docs/level04.png) · [Level 5](Docs/level05.png).

The survival clock pauses outside the arena and resets when a life is lost. Countdown timers continue during respawn. Restart resets the level's timer, coins, hearts and lives.

## Gameplay

The player has three hearts, one-second damage protection with blinking feedback, knockback and protected checkpoint respawn. Falling or losing all hearts costs a life. Running out of lives or time displays the lose panel. Winning and losing freeze time; restarting restores it before loading the scene.

Guards patrol waypoints, chase nearby players and return after giving up. Crushers warn before striking. Speed potions give a five-second 1.6× boost; a second pickup refreshes the timer without stacking speed.

The five layouts are reused from [Assignment 03](https://github.com/arty542/CS464-A3-bscs23204), with different objective rules and difficulty for this lab. Environment assets are from the instructor-provided Synty packages; the HUD uses TextMesh Pro.
