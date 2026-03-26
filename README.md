# Ultimate Hybrid Game

## Fighting + Driving + Action

The ultimate hybrid game combining intense combat, high-speed driving, and epic action!

![Version](https://img.shields.io/badge/version-2.0-blue)
![Unity](https://img.shields.io/badge/Unity-2022.3-black)
![License](https://img.shields.io/badge/license-MIT-green)
![Made by](https://img.shields.io/badge/Made%20by-Bertin%20ABIJURU-orange)

**Created by Bertin ABIJURU** | [GitHub](https://github.com/Bertin-26) | [Instagram](https://instagram.com/bertin_tonic880)

---

## 🎮 Features

### Combat System
- **Punch & Kick**: Basic attacks with combo system
- **Special Moves**: Power Punch (Q), Roundhouse Kick (E), Ground Slam (R), Ultimate (F)
- **Dash/Roll**: Quick dodges with invincibility frames
- **Combo System**: Chain hits for bonus damage (x2 at 3 hits, x3 at 5 hits)
- **Smooth Movement**: Acceleration and deceleration for professional feel

### Vehicle System
- **Driving**: Full physics-based driving with WASD
- **Weapons**: Machine Gun, Rockets, Mines
- **Abilities**: Shield, Nitrous Boost, Ram Attack
- **Enter/Exit**: Dynamic switching between on-foot and vehicle

### Power-Up System 🆕
| Power-Up | Color | Effect |
|----------|-------|--------|
| Health | Red | Restores 25 HP |
| Ammo | Blue | Refills all weapons |
| Speed | Cyan | 5-second speed boost |
| Shield | Magenta | Temporary shield |
| Score | Yellow | +100 bonus points |
| Invincible | White | 3-second invincibility |

Power-ups spawn randomly when enemies are defeated (30% chance)! Look for floating, spinning orbs!

### Enemy Types
| Type | Health | Speed | Special |
|------|--------|-------|---------|
| Normal | 50 | Medium | Basic melee |
| Fast | 30 | Fast | Quick attacks |
| Heavy | 150 | Slow | High damage |
| Ranged | 40 | Slow | Shoots projectiles |
| Explosive | 35 | Medium | Explodes on death |
| Boss | 500 | Slow | Heals, summons, area attacks |

### Visual Effects
- Smooth movement with acceleration/deceleration
- Dust particles when running fast
- Particle effects for all actions
- Camera shake on damage and explosions
- Post-processing (Bloom, Motion Blur, Vignette)
- Speed lines and trails
- Power-up floating animation (spin + bob)

### Audio
- Dynamic music (Gameplay, Combat, Boss, Vehicle)
- 40+ sound effects
- 3D positional audio
- Full volume control
- Power-up pickup sounds

### Save System
- Persistent high scores
- Achievement system (7 achievements)
- Settings save (volume, graphics, controls)
- Play time tracking

### Leaderboard System 🆕
- Top 5 high scores saved locally
- 🥇 Gold, 🥈 Silver, 🥉 Bronze medals for top 3
- Press **Tab** to view leaderboard anytime
- Auto-saves on game over

---

## 🎮 Controls

### On-Foot
| Key | Action |
|-----|--------|
| WASD | Move |
| Shift | Run |
| Shift + Direction | Dash Attack |
| Space | Jump |
| Left Mouse | Punch |
| Right Mouse | Kick |
| Q | Power Punch |
| E | Roundhouse Kick |
| R | Ground Slam |
| F | Ultimate |
| C / Ctrl | Roll |

### Vehicle
| Key | Action |
|-----|--------|
| E (near) | Enter Vehicle |
| F (inside) | Exit Vehicle |
| WASD | Drive |
| Space | Brake |
| Left Mouse | Machine Gun |
| Right Mouse | Rocket |
| Q | Drop Mine |
| F | Ram Attack |
| E | Shield |
| Shift | Nitrous Boost |
| 1,2,3 | Switch Weapons |

### UI
| Key | Action |
|-----|--------|
| Esc | Pause Menu |
| Tab | Toggle Leaderboard |

---

## 🏆 Achievements

- **First Blood** - Defeat 10 enemies
- **Enemy Slayer** - Defeat 100 enemies
- **Legendary Warrior** - Defeat 500 enemies
- **Wave Survivor** - Complete 5 waves
- **Wave Master** - Complete 10 waves
- **Wave Legend** - Complete 20 waves
- **Game Complete** - Beat all 20 waves

---

## 🛠️ Built With

- Unity Engine
- C# Scripting
- Post-Processing Stack

---

## 📁 Scripts (15 total)

| Script | Description |
|--------|-------------|
| PlayerController.cs | Player movement, combat, combos, power-ups |
| VehicleController.cs | Vehicle driving, weapons, abilities |
| Enemy.cs | Enemy AI with 6 types |
| GameManager.cs | Game state, wave system, power-up spawning |
| UIManager.cs | Complete HUD + Leaderboard system |
| CameraController.cs | Dynamic camera with shake and zoom |
| AudioManager.cs | Audio management with volume control |
| SaveSystem.cs | Save/load system with achievements |
| EffectManager.cs | Particle effects with object pooling |
| SpawnManager.cs | Wave-based enemy spawning |
| PowerUp.cs | 6 power-up types with animations |
| Projectile.cs | Basic projectiles |
| RocketProjectile.cs | Explosive rocket projectiles |
| Mine.cs | Deployable mine system |
| PostProcessingSetup.cs | Post-processing effects setup |

---

## 🚀 How to Play

1. Import all scripts into Unity
2. Set up scene with Player, Ground, and Enemies
3. Create Power-Up prefabs (6 colors, floating animation)
4. Assign references in UIManager
5. Press Play and enjoy!

---

## 🔗 Links

- [GitHub Repository](https://github.com/Bertin-26/UltimateHybridGame)
- [Follow on Instagram](https://instagram.com/bertin_tonic880)

---

## 📝 What's New in Version 2.0

- ✨ **Smooth Movement** - Acceleration and deceleration for professional feel
- 💪 **6 Power-Up Types** - Health, Ammo, Speed, Shield, Score, Invincible
- 🏆 **Leaderboard System** - Top 5 high scores with medals
- 🎨 **Visual Polish** - Dust particles, floating power-ups, color coding
- 🔧 **Performance** - Object pooling for effects

---

## 👨‍💻 Developer

**Created by: Bertin ABIJURU**

[![GitHub](https://img.shields.io/badge/GitHub-Bertin--26-181717?style=flat&logo=github)](https://github.com/Bertin-26)
[![Instagram](https://img.shields.io/badge/Instagram-@bertin_tonic880-E4405F?style=flat&logo=instagram)](https://instagram.com/bertin_tonic880)

---

*Built with passion. Code with purpose.*

© 2026 Bertin ABIJURU. All rights reserved.