# Twin-Stick Shooter

A stylized 3D twin-stick shooter built in Unity as a **portfolio project** to demonstrate gameplay
programming, system architecture, and Unity tooling craft.

> ## 🚧 Work In Progress
>
> **This project is under active development and is not a finished game.** Systems are being built and
> refined iteratively. Some features are partial, placeholder art is in use, and content (levels, enemies,
> progression) is still being assembled. What's here is meant to showcase **engineering and architecture
> quality**, not a complete, shippable product.

---

## At a Glance

| | |
|---|---|
| **Engine** | Unity 6000.3.8f1 |
| **Language** | C# (.NET Standard 2.1) |
| **Render Pipeline** | Universal Render Pipeline (URP) 17.3.0 |
| **Genre** | Twin-stick shooter, isometric view |
| **Art style** | 3D |
| **Target platforms** | Windows |

---

## Engineering Highlights

The project is organized as self-contained systems under `Assets/_Project/<SystemName>/`, with clear seams
between them. Highlights a reviewer might care about:

- **Layered finite state machines.** The player is driven by separate **locomotion** and **combat** state
  machines (`PlayerFiniteStateMachine/`), each with its own blackboard and discrete states
  (free-move / aim-move, idle / aimed / reload). State logic is decoupled from the controller via a
  `PlayerBrain` coordinator.
- **Weapon system with procedural aim IK.** Weapons use Animation Rigging for spine/aim IK driven by a
  data-authored `WeaponIKProfile` ScriptableObject, plus aim-line rendering and weapon-pickup stands.
- **Designer-first data authoring.** Gameplay tunables live on ScriptableObjects and serialized fields,
  not magic numbers — values are editable in the Inspector without recompiling.
- **Object pooling** for short-lived objects (projectiles, enemies, VFX) to keep allocations down.
- **Decoupled cross-system communication** via the Observer pattern and interface-based service seams,
  keeping systems independently testable and replaceable.

### Patterns & techniques on display

- Finite State Machine (layered, blackboard-driven)
- Object Pooling
- Factory Pattern
- Observer / event-driven messaging
- Strategy via ScriptableObject configuration
- Procedural animation (Animation Rigging IK)

### Systems in the project

`PlayerCharacter` · `PlayerFiniteStateMachine` · `MovementSystem` · `WeaponSystem` · `HealthSystem` ·
`AnimationSystem` · `InteractionSystem` · `HUD` · `InputSystem` · `ObjectPooling` · `StateMachineSystem`

---

## Custom Packages (AKD)

Reusable, self-contained packages authored for this project, kept separate from game code under
`Assets/_AKDPackages/`:

- **AKD.AnimationEvents** — a Mecanim animation-event framework that fires events on *state entered*, *a
  moment inside a state*, *clip finished*, or *state left* — **without hand-placing events on individual
  AnimationClips**. Supports state / tag / any-state scopes, per-layer targeting, an Inspector-driven
  designer workflow, and a code API. *This package was designed and implemented with the help of
  [Claude Code](https://www.anthropic.com/claude-code) as an AI pair-programming collaborator.* See
  [`Assets/_AKDPackages/AKD.AnimationEvents/README.md`](Assets/_AKDPackages/AKD.AnimationEvents/README.md)
  for full documentation.
- **AKD.Shaders** — custom shader work for the project's visual style.

---

## Third-Party Packages & Assets

### System packages

- **PrimeTween** — lightweight tweening for game feel

### Art & animation assets

- **POLYGON – Prototype Pack** (3D) — *placeholder / prototype art while systems are built*
- **Human Soldier Animations (FREE)**
- **Mixamo** (animations)
- **Omni Animation – Core Locomotion Pack**

> Third-party art is currently used as prototype/placeholder content and may be replaced as the project
> matures.
