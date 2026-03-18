# ZooWorld

A Unity ecosystem simulator - predators (Snakes) hunt prey (Frogs) inside a bounded arena. Animals move autonomously and compete for survival while kill counts are tracked on the HUD.

## Tech Stack

- **Unity 6**
- **Zenject** - dependency injection
- **UniRx** - reactive programming
- **TextMeshPro**
- **ScriptableObjects** - data-driven configuration

## Architecture

MVP pattern with Zenject DI and UniRx reactive streams.

```
AnimalModel      - pure data (type, alive state, stats)
AnimalView       - MonoBehaviour, owns Rigidbody & Collider
AnimalPresenter  - wires model <-> view, drives physics tick
```

**Design patterns used:**

- **Strategy** - `IMovementStrategy` per animal type
  - `JumpMovementStrategy` - parabolic arc jumps (Frog)
  - `LinearMovementStrategy` - smooth steering (Snake)
- **Object Pool** - `BasePool<T>` extended by `AnimalPool` and `TastyLabelPool`
- **Factory** - `AnimalFactory` creates fully-wired presenters via Zenject
- **Registry** - `AnimalRegistry` tracks all live animals
- **Services** - `AnimalSpawnService`, `ScoreService`, `GameBoundsService`, `GameService`

## Configuration

All values are ScriptableObjects in `Assets/_Project/Configs/` and can be tuned from the Inspector.

**FrogConfig**
| Parameter | Value |
|---|---|
| Jump Interval | 1.5 s |
| Jump Duration | 0.5 s |
| Jump Distance | 2.0 units |
| Jump Height | 0.5 units |

**SnakeConfig**
| Parameter | Value |
|---|---|
| Direction Change Interval | 2 s |
| Rotation Speed | 5 |
| Max Turn Angle | 60° |
| Velocity Damping | 15 |

**SpawnLimitsConfig**
| Parameter | Value |
|---|---|
| Max Prey | 25 |
| Max Predators | 3 |
| Min Spawn Distance | 2.0 units |

## Game Mechanics

**Eating** - a snake eats a frog only on a head-on collision, determined by XZ dot product `> 0.2`.

**Predator vs predator** - when two snakes collide, the winner is decided by `GetHashCode()` comparison - deterministic and reproducible.

**Bounds** - arena size is derived from the camera's orthographic size via `GameBoundsService`. Animals reflect off walls and are clamped inside.

**Spawning** - animals spawn every 1-2 seconds at a random position. `Physics.OverlapSphereNonAlloc` prevents overlapping spawns.

**TastyLabel** - a label floats above the snake after a kill and auto-hides after 1.5 s. Labels are pooled via `TastyLabelPool`.

**Restart** - clears all active animals, resets the score, and restarts the spawn loop without reloading the scene.

## Project Structure

```
Assets/_Project/
├── Animals/
│   ├── Abstractions/       # IAnimal, IMovementStrategy, ICollisionHandler
│   ├── Core/               # AnimalModel, AnimalView, AnimalPresenter
│   ├── CollisionHandlers/  # PreyCollisionHandler, PredatorCollisionHandler
│   └── Movement/           # JumpMovementStrategy, LinearMovementStrategy
├── Configs/                # AnimalConfig, FrogConfig, SnakeConfig, SpawnLimitsConfig
├── Infrastructure/
│   ├── Factories/          # AnimalFactory
│   ├── Pools/              # BasePool<T>, AnimalPool, TastyLabelPool
│   └── Services/           # AnimalSpawnService, AnimalRegistry, GameBoundsService, ScoreService, GameService
├── Installers/             # SceneInstaller
└── Ui/                     # HudView, TastyLabel
```

## Getting Started

**Prerequisites:**
- Unity 6.x
- [Zenject](https://github.com/modesttree/Zenject)
- [UniRx](https://github.com/neuecc/UniRx)

**Setup:**
1. Clone the repository:
   ```bash
   git clone https://github.com/OOpipoo/zoo-world.git
   ```
2. Open the project in Unity 6.
3. Open `Assets/_Project/Scenes/GameScene.unity` and press Play.

## License

MIT
