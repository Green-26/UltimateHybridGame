# Unity Setup Instructions

## 🎮 Ultimate Hybrid Game - Unity Import Guide

Follow these steps to set up your game in Unity.

---

## 📋 Prerequisites

- Unity 2022.3 LTS or newer
- Basic knowledge of Unity Editor

---

## Step 1: Create Unity Project

1. Open **Unity Hub**
2. Click **New Project**
3. Select **3D Core** template
4. Name project: `UltimateHybridGame`
5. Choose location and click **Create**

---

## Step 2: Import Scripts

1. In Unity, go to **Assets** folder
2. Create folder: `Scripts`
3. Copy ALL 14 scripts from your GitHub repository:
   - PlayerController.cs
   - VehicleController.cs
   - Enemy.cs
   - GameManager.cs
   - UIManager.cs
   - CameraController.cs
   - AudioManager.cs
   - SaveSystem.cs
   - EffectManager.cs
   - SpawnManager.cs
   - Projectile.cs
   - RocketProjectile.cs
   - Mine.cs
   - PostProcessingSetup.cs

4. Paste into `Assets/Scripts/` folder
5. Wait for Unity to compile scripts

---

## Step 3: Create Game Objects

### 3.1 Create Player

| Action | Instructions |
|--------|--------------|
| GameObject | Right-click Hierarchy → 3D Object → Capsule |
| Name | "Player" |
| Tag | Add tag "Player" (create new if needed) |
| Position | (0, 1, 0) |
| Components | Add Rigidbody, Add PlayerController |

### 3.2 Create Ground

| Action | Instructions |
|--------|--------------|
| GameObject | Right-click → 3D Object → Plane |
| Name | "Ground" |
| Tag | Add tag "Ground" |
| Scale | (10, 1, 10) |
| Position | (0, 0, 0) |

### 3.3 Create Camera

| Action | Instructions |
|--------|--------------|
| GameObject | Main Camera (already exists) |
| Name | "Main Camera" |
| Position | (0, 2, -5) |
| Components | Add CameraController |
| Target | Drag Player into CameraController's Target slot |

### 3.4 Create Enemies (Prefabs)

**For each enemy type, create a prefab:**

1. Create GameObject → 3D Object → Capsule
2. Name: `Enemy_Normal`, `Enemy_Fast`, `Enemy_Heavy`, etc.
3. Add Component → Enemy
4. Set Enemy Type in Inspector
5. Adjust scale:
   - Normal: (1,1,1)
   - Fast: (0.8,0.8,0.8)
   - Heavy: (1.5,1.5,1.5)
   - Boss: (2,2,2)
6. Drag into Assets/Prefabs folder to create prefab

### 3.5 Create Vehicle

| Action | Instructions |
|--------|--------------|
| GameObject | Right-click → 3D Object → Cube |
| Name | "Vehicle" |
| Scale | (2, 0.5, 4) |
| Components | Add Rigidbody, Add VehicleController |

### 3.6 Create Spawn Manager

| Action | Instructions |
|--------|--------------|
| GameObject | Create Empty |
| Name | "SpawnManager" |
| Position | (0, 0, 0) |
| Components | Add SpawnManager |
| Assign | Drag all enemy prefabs into slots |

---

## Step 4: Create UI Canvas

### 4.1 Create Canvas

1. GameObject → UI → Canvas
2. Name: "UI"
3. Add Component → UIManager

### 4.2 Create UI Elements

Create these UI elements as children of Canvas:

| Element | Type | Purpose |
|---------|------|---------|
| HealthBar | Slider | Player health |
| ScoreText | Text | Score display |
| ComboText | Text | Combo counter |
| MachineGunAmmo | Text | Ammo count |
| RocketAmmo | Text | Rocket count |
| MineAmmo | Text | Mine count |
| PowerPunchIcon | Image | Special move cooldown |
| VehiclePanel | Panel | Vehicle UI container |
| SpeedText | Text | Vehicle speed |
| Crosshair | Image | Aim reticle |

### 4.3 Assign UIManager References

In UIManager Inspector, drag all UI elements into their corresponding slots.

---

## Step 5: Create Audio System

| Action | Instructions |
|--------|--------------|
| GameObject | Create Empty |
| Name | "AudioManager" |
| Components | Add AudioManager |
| Assign | Drag all audio clips into Inspector slots |

---

## Step 6: Create Save System

| Action | Instructions |
|--------|--------------|
| GameObject | Create Empty |
| Name | "SaveSystem" |
| Components | Add SaveSystem |

---

## Step 7: Create Effect Manager

| Action | Instructions |
|--------|--------------|
| GameObject | Create Empty |
| Name | "EffectManager" |
| Components | Add EffectManager |
| Assign | Drag all particle prefabs into slots |

---

## Step 8: Set Up Lighting

1. GameObject → Light → Directional Light
2. Position: (0, 10, 0)
3. Rotation: (50, -30, 0)
4. Intensity: 1

---

## Step 9: Post-Processing (Optional)

1. Window → Package Manager
2. Install **Post Processing** package
3. Add PostProcessVolume to Camera
4. Create PostProcessProfile and assign effects

---

## Step 10: Test the Game

1. Click **Play** button
2. Test movement: WASD
3. Test combat: Left Mouse (Punch), Right Mouse (Kick)
4. Test special moves: Q, E, R, F
5. Test dash: Shift + Direction
6. Test roll: C or Ctrl
7. Enter vehicle: E near vehicle
8. Drive: WASD
9. Vehicle weapons: Left Mouse, Right Mouse, Q, F

---

## 🔧 Troubleshooting

### Script Errors

| Error | Solution |
|-------|----------|
| Missing namespace | Add `using UnityEngine;` at top of scripts |
| NullReferenceException | Assign all references in Inspector |
| Missing Tag | Create tags: Player, Enemy, Ground |

### Audio Not Playing

- Ensure AudioManager exists in scene
- Assign all audio clips in Inspector
- Check volume settings

### UI Not Showing

- Ensure Canvas exists
- Assign all references in UIManager
- Check UI Layer is visible

### Enemies Not Spawning

- Assign enemy prefabs to SpawnManager
- Check spawn points are set
- Verify tags are correct

### Vehicle Not Working

- Ensure Rigidbody is attached
- Check isPlayerInside becomes true
- Verify input keys

---

## ✅ Final Checklist

- [ ] All 14 scripts imported
- [ ] Player created with PlayerController
- [ ] Ground created with Ground tag
- [ ] Camera with CameraController
- [ ] Enemy prefabs created for all 6 types
- [ ] Vehicle created with VehicleController
- [ ] SpawnManager with enemy prefabs
- [ ] UI Canvas with UIManager
- [ ] AudioManager with audio clips
- [ ] SaveSystem in scene
- [ ] EffectManager with particle effects
- [ ] Lighting set up
- [ ] Game runs without errors

---

## 🎮 Ready to Play!

Once everything is set up, press Play and enjoy your Ultimate Hybrid Game!

**Questions?** Check the [GitHub Repository](https://github.com/YOURUSERNAME/UltimateHybridGame)

---

*Happy Gaming!* 🚀