# Project Overview: Tugas-Game (Dark Knight Project)

This project is a 2D action platformer built in Unity, featuring a character controller for a "Dark Knight" protagonist, enemy AI systems (Skeleton), and a level progression structure. It utilizes the Universal Render Pipeline (URP) for 2D lighting and visual effects, and integrates assets from the TealFalcon Enemy Series.

## 1. Project Description
The project is a 2D side-scrolling action game where players control a Dark Knight character to navigate levels and fight enemies. It is designed as a technical showcase of character animation, state-based control, and 2D combat mechanics. 
**Core Pillars:**
- **State-Driven Combat:** Precise control over attacking, guarding, and movement states.
- **Dynamic 2D Visuals:** Use of URP, custom ShaderGraph effects (dissolve, glow), and frame-synced animation events.
- **Classic Platforming:** Physics-based movement with standard jump and run mechanics.

## 2. Gameplay Flow / User Loop
1.  **Entry:** The game begins in a home or level selection screen (`Home.unity` or `LevelSelect.cs`).
2.  **Selection:** Players choose a level (Level 1, Level 2) through a UI-driven level select system.
3.  **Active Gameplay:**
    - Player controls the Dark Knight using horizontal input (Walk/Run), Space (Jump), and Mouse (Guard/Attack).
    - Engagement with AI enemies (Skeletons) that chase and attack within range.
    - Health management: Players take damage from enemies and can reach a "Death" state which triggers visual dissolve effects.
4.  **Transitions:** Winning or losing leads back to level selection or restarts the level via `LevelSelect.cs`.

## 3. Architecture
The project follows a **Component-Based Architecture** with a clear separation between Input, Logic, and Visuals.
- **Input Layer:** `PlayerInput.cs` captures hardware input and commands the controller.
- **Logic/Controller Layer:** `DarkKnightController.cs` acts as the primary State Machine for the player character.
- **Health/Lifecycle Layer:** `PlayerHealth.cs` manages the vital stats and triggers death sequences in the controller.
- **AI Layer:** `SkeletonAI.cs` independently manages enemy behavior using proximity-based logic.
- **Data Flow:** Input -> Controller -> Animator/Rigidbody2D. Events (UnityEvents) are used to trigger sound and particle effects during state changes (Hurt, Death).

## 4. Game Systems & Domain Concepts

### Character Controller System
A state-machine driven controller that handles complex animations and physics interactions for the main character.
- `DarkKnightController`: Manages `MovementState` (Idle, Walking, Running) and `FightingState` (Attacking, Guarding, Hurt, Death).
- `PlayerInput`: Bridges Unity's Legacy Input Manager to the controller's methods.
- **Design Pattern:** State Machine (Enum-based).
- **Extension:** Add new states to the `FightingState` enum and implement corresponding routines in `DarkKnightController.cs`.
- `Location: Assets/Dark Knight/Scripts/`

### AI Combat System
A proximity-based behavioral system for enemies.
- `SkeletonAI`: Handles player detection (chase range vs. attack range) and executes physics-based movement.
- `OnAttackHitEvent`: Uses Animation Events to trigger a `Physics2D.OverlapCircle` check for damage synchronization.
- **Design Pattern:** Proximity-based State Logic (Chasing/Attacking).
- **Extension:** Subclass or duplicate `SkeletonAI` to define new detection ranges or attack patterns.
- `Location: Assets/` (Root)

### Health & Damage System
Manages the lifecycle of game entities.
- `PlayerHealth`: Stores HP, handles damage intake, and interfaces with the `DarkKnightController` for visual feedback (Hurt/Death).
- `TakeDamage(int damage)`: The primary entry point for reducing health and checking death conditions.
- `Location: Assets/` (Root)

### Camera System
A smooth-follow system to keep the player in view.
- `CameraFollow`: Uses `Vector3.Lerp` to follow a target transform with a configurable offset.
- `Location: Assets/` (Root)

## 5. Scene Overview
- `Home.unity` / `Home 1.unity`: Initial entry points, likely serving as the Main Menu.
- `Level1.unity`: The primary gameplay level containing the player, environment, and enemies.
- `Level2.unity`: A secondary level for progression testing.
- `Demo.unity`: A showcase scene for the Dark Knight asset's capabilities.
- **Scene Flow:** Managed by `LevelSelect.cs`, which uses `SceneManager.LoadScene`.

## 6. UI System
The project uses **UGUI** (Unity UI) for its interface.
- `PopUpCode.cs`: Likely handles UI overlays or instructional popups.
- `LevelSelect.cs`: Manages button interactions for scene transitions.
- **Assets:** UI textures are stored in `TooCubeForest/images` and `UI-Assets/`, featuring health bar sprites and menu backgrounds.
- `Location: Assets/Script/`

## 7. Asset & Data Model
- **Prefabs:** Character variants (e.g., `Dark Knight.prefab`, `SkeletonWarrior.prefab`) are located in their respective character folders.
- **Materials/Shaders:** Custom `ShaderGraph` files (`DarkKnightShader.shadergraph`) control specialized effects like `_Dissolve` and `_Glow`.
- **ScriptableObjects:** Rendering settings are stored in `Assets/Settings/` (URP Assets).
- **Animations:** Animator Controllers use parameters like `Speed`, `Busy`, `Guard`, and `Hurt` to drive transitions.
- **Naming Convention:** Scripts are generally PascalCase; asset folders are categorized by type (Scripts, Prefabs, Materials).

## 8. Notes, Caveats & Gotchas
- **Animation Sync:** Combat damage is strictly tied to **Animation Events**. If you change the Skeleton's attack animation without moving the `OnAttackHitEvent`, the damage timing will be visually broken.
- **Physics Dependencies:** The `DarkKnightController` requires a `Rigidbody2D` with specific constraints (Freeze Rotation) to function correctly without tipping over.
- **Input System:** The project currently supports both the New Input System and Legacy Manager, but `PlayerInput.cs` specifically relies on `Input.GetAxisRaw` (Legacy).
- **Material Blocks:** The `DarkKnightController` uses `MaterialPropertyBlock` for dissolve effects. Manual changes to materials in the Inspector may be overridden by the script at runtime.
- **Tag Dependency:** `SkeletonAI` looks for the player using the "Player" tag. If the player prefab is not tagged, the AI will fail to function.