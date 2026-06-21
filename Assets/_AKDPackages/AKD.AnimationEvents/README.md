# AKD.AnimationEvents

Animation events for Mecanim **without hand-placing events on AnimationClips**. Subscribe to *state entered*, *a moment inside the state*, *clip finished*, or *state left* — from code or entirely in the Inspector — and target a single state, **every state sharing a tag**, or **any state at all**, optionally restricted to one animator layer.

Typical wins:

- "Spawn the magazine at 40% of the reload animation" — one slider, no clip editing.
- "Run this when **any** weapon's reload finishes" — tag your reload states `Reload` once; new weapons need **zero new code**.
- "Play footsteps at 30% and 80% of Run, but only on the base layer."

---

## Quick Start (5 steps)

1. **Add the dispatcher.** Select the GameObject that has the `Animator` → *Add Component* → **Animation Event Dispatcher**.
2. **Wire the controller.** In the dispatcher's Inspector, click **Setup Controller**. This installs the event relay on every animator layer (see [Setup Controller](#setup-controller-button)). Without this step **no events ever fire** — the Inspector warns you if it's missing.
3. *(Optional, for tag-based events)* **Tag your states.** Open the Animator window, select a state, and set its **Tag** field in the Inspector (e.g., tag every reload state `Reload`).
4. **Subscribe.**
   - *Designer path:* add an **Animation Event Listener** component anywhere, assign the dispatcher, click **Add Binding** and configure it (see [Listener Inspector](#animation-event-listener)).
   - *Code path:* call `dispatcher.Register(...)` — see [Scripting API](#scripting-api).
5. **Press Play.** Events fire as the animator runs.

---

## Core Concepts

| Piece | What it is |
|---|---|
| `AnimationEventDispatcher` | Component next to the `Animator`. The hub: owns all registrations, fans events out to matching subscribers. Everything talks to this. |
| `AnimationEventBehaviour` | The relay — a `StateMachineBehaviour` that *Setup Controller* installs on each layer's root state machine. It watches state lifecycles and reports to the dispatcher. You never add or script against it directly. |
| `AnimationEventListener` | Optional component for designer-authored subscriptions: a list of **bindings**, each pairing a scope + event type with a `UnityEvent` response. |
| **Scope** | *What* you listen to × *where*: (specific **State** \| every state with a **Tag** \| **Any** state) × (one **layer** \| **any layer**). |

### Event types

| Event | Fires when | Notes |
|---|---|---|
| **OnStart** | The state is entered | |
| **OnTime** | The state's normalized time crosses your threshold (0–1) | Fires once per loop cycle. A threshold of exactly **0 never fires** — that moment is `OnStart` (the slider bottoms out at 0.001). |
| **OnComplete** | The clip reaches its end *naturally* | Once per cycle on looping states, once on one-shots. **Never fires if the state is interrupted.** ⚠ A transition with *Has Exit Time* and **Exit Time < 1** leaves the state before the clip mathematically completes — `OnComplete` will not fire. Set Exit Time to 1, or use `OnExit`. |
| **OnExit** | The state is left for **any** reason | Natural finish *and* interruption (e.g., reload cancelled by weapon switch). |

**Rule of thumb:** want "the animation finished doing its job"? → `OnComplete`. Want "we're no longer in this state"? → `OnExit`.

---

## Inspector Reference

### Animation Event Dispatcher

| Control | What it does |
|---|---|
| **Ignore Zero Weight Layers** (default **on**) | A state *entered* while its layer weight is 0 raises no events at all for that play-through. Decided once at entry, so an `OnStart` never loses its matching `OnExit` when weights animate mid-state. The base layer (index 0) is always active. Turn **off** if you deliberately run invisible "logic layers" at weight 0 and want their events. |
| **Setup Controller** button | See below. |
| Warning boxes | *"…no AnimationEventBehaviour on its root state machine"* → events on that layer can never fire; click Setup Controller. *"…per-state AnimationEventBehaviours (legacy setup)"* → old hand-placed relays exist; Setup Controller cleans them up. |

#### Setup Controller button

One click, idempotent (running it twice changes nothing), undoable (Ctrl+Z):

- Adds the `AnimationEventBehaviour` relay to **every layer's root state machine** — one relay covers all states inside that layer, including sub-state machines.
- Removes stray relays hand-placed on individual states.
- **Skips synced layers** — they borrow the source layer's state machine and inherit its relay automatically.

Re-run it after adding new layers to the controller.

### Animation Event Listener

| Field | What it does |
|---|---|
| **Dispatcher** | The `AnimationEventDispatcher` to subscribe through. All dropdowns below populate from *its* AnimatorController. |
| **Add Binding** | Appends a new binding (defaults to *Any Layer*). |

Each **binding** shows:

| Field | What it does |
|---|---|
| **Scope** | `State` — one specific state. `Tag` — every state carrying a tag. `Any` — every state. |
| **Layer** | `Any Layer` (default) — match on every layer. Or pick a named layer to restrict matching to it. |
| **State** (Scope = State) | Dropdown of the controller's states. With a specific Layer selected, entries show `Layer/State` paths and only that layer's states. With *Any Layer*, the list is **deduplicated by name** — one entry matches that state on every layer. Synced layers list their source layer's states. |
| **Tag** (Scope = Tag) | Dropdown of every distinct tag found in the controller. If empty: no states are tagged yet — tag them in the Animator window first. |
| **Event Type** | `OnStart` / `OnTime` / `OnComplete` / `OnExit` (semantics above). |
| **Normalized Time** (OnTime only) | The 0.001–1 threshold. The ▶ **preview button** (State scope only) poses the model in the Scene view at the slider's time — scrub to find the exact frame, no play mode needed. Press again to stop. |
| **Response** | A standard `UnityEvent` — hook up any method like a Button's OnClick. |
| **Remove** | Deletes the binding. |

**Stale-binding warning:** if a bound state or tag no longer exists (renamed/removed in the controller), the binding shows a warning box — re-select it. Bindings match by name hash, so renames silently break them; this warning is your detector.

---

## Scoping Up: from one state to a whole category

The intended progression as your project grows:

1. **One state** — `Scope: State`, e.g. `Reload_Pistol`. Fine for one-offs.
2. **A category of states** — tag every reload state `Reload` in the controller, switch the binding/registration to `Scope: Tag`. Adding a Shotgun later = tag its reload state. **No code, no new bindings.**
3. **Layer-restricted** — any scope can additionally pin one layer (e.g., tag `Reload` *on the Pistol layer only*) via the Layer dropdown / `layer:` parameter.
4. **Everything** — `Scope: Any` fires for every state; the callback's context tells you which (great for debug overlays, footstep systems driven by tags, etc.).

---

## Scripting API

All registration goes through the dispatcher. Pattern: `Register` in `OnEnable`, mirrored `Unregister` in `OnDisable`.

### Building a scope

```csharp
AnimationEventScope.State("Reload")               // this state, any layer
AnimationEventScope.State("Reload", layer: 2)     // this state, layer 2 only
AnimationEventScope.Tag("Reload")                 // any state tagged "Reload"
AnimationEventScope.Tag("Reload", layer: 0)       // tagged, base layer only
AnimationEventScope.Any()                         // every state, every layer
AnimationEventScope.Any(layer: 1)                 // every state on layer 1
```

⚠ State names are **short names** (`"Reload"`), never full paths (`"Base Layer.Reload"` will silently never match). Tags must match the state's Tag field exactly (case-sensitive).

### Registering

```csharp
// OnStart / OnComplete / OnExit:
dispatcher.Register(scope, AnimationEventType.OnComplete, MyCallback);

// OnTime — pass the threshold instead of the event type:
dispatcher.Register(scope, 0.4f, MyCallback);

// Mirror both with Unregister(...) using the same arguments + callback.
```

Callbacks receive an `AnimationEventContext`:

| Field | Meaning |
|---|---|
| `Animator` | The animator that raised the event |
| `StateHash` | `shortNameHash` of the state (compare against cached `Animator.StringToHash("Name")`) |
| `TagHash` | The state's tag hash, `0` if untagged |
| `LayerIndex` | Which layer fired — essential with Any-Layer scopes |
| `EventType` | Which of the four events this is |
| `NormalizedTime` | The crossed threshold for `OnTime`, `1` for `OnComplete`, entry/exit time for `OnStart`/`OnExit` |

### Complete example

```csharp
using AKD.AnimationEvents;
using UnityEngine;

public class WeaponReloadWatcher : MonoBehaviour
{
    [SerializeField] private AnimationEventDispatcher dispatcher;

    // Cache hashes once — never hash strings per frame.
    private static readonly int ReloadTag = Animator.StringToHash("Reload");

    private void OnEnable()
    {
        // Any weapon's reload finished, on any layer:
        dispatcher.Register(AnimationEventScope.Tag(ReloadTag), AnimationEventType.OnComplete, OnReloadComplete);
        // Magazine-out moment at 40% of the same animations:
        dispatcher.Register(AnimationEventScope.Tag(ReloadTag), 0.4f, OnMagazineOut);
    }

    private void OnDisable()
    {
        dispatcher.Unregister(AnimationEventScope.Tag(ReloadTag), AnimationEventType.OnComplete, OnReloadComplete);
        dispatcher.Unregister(AnimationEventScope.Tag(ReloadTag), 0.4f, OnMagazineOut);
    }

    private void OnReloadComplete(AnimationEventContext ctx)
        => Debug.Log($"Reload finished on layer {ctx.LayerIndex}");

    private void OnMagazineOut(AnimationEventContext ctx)
        => Debug.Log($"Drop magazine at t={ctx.NormalizedTime:F2}");
}
```

Good to know:

- **Registering mid-state works** — a threshold registered while the state is already playing still fires this play-through (if not already passed).
- **Unregistering inside a callback is safe**, including unregistering yourself.
- Duplicate `Register` calls with the same scope/event/callback are ignored (no double-fires from double-subscribes).
- `Register(scope, AnimationEventType.OnTime, cb)` is rejected with a warning — `OnTime` needs the threshold overload.

---

## Behaviour details & gotchas

- **Duplicates across layers:** an Any-Layer scope fires **once per layer**. Synced layers mirror their source's states, so both fire the same moment — intentional (each layer really played it); `ctx.LayerIndex` tells them apart. Avoid via a layer-scoped registration, branching on `ctx.LayerIndex`, or zero-weight muting.
- **Synced layers** are fully supported: dropdowns list the source layer's states under the synced layer's name, events carry the synced layer's index, Setup Controller knows to skip them.
- **Interrupted states:** `OnTime` thresholds not yet reached when a state is interrupted are **not** fired retroactively — by design (a cancelled reload shouldn't spawn its magazine). You always still get `OnExit`.
- **Frame spikes** that skip whole loop cycles fire `OnComplete` once (not per skipped cycle) and don't replay skipped `OnTime` thresholds.
- **Not supported:** reverse playback (negative speed), per-clip events inside blend trees (events are per *state*), preview on blend-tree states.
- **Performance:** dispatch is allocation-free; the relay does no per-frame string work. Cache your hashes (`static readonly int`) when registering from code.

## Troubleshooting

| Symptom | Likely cause |
|---|---|
| Nothing ever fires | **Setup Controller** never clicked (dispatcher Inspector warns), or no `AnimationEventDispatcher` on the Animator's GameObject (Console warning at runtime). |
| Fires for the wrong/extra layer | Any-Layer scope + multiple layers playing the state — scope the layer or check `ctx.LayerIndex`. |
| `OnComplete` never fires | The state leaves via *Has Exit Time* with Exit Time **< 1**, or gets interrupted. Use `OnExit`, or set Exit Time to 1. |
| Events stopped after setting a layer weight to 0 | **Ignore Zero Weight Layers** is on (default) — intended; turn it off for logic layers. |
| Code registration never matches | Full path used instead of the short state name, or tag string case mismatch. |
| Binding shows a warning box | The state/tag was renamed or removed in the controller — re-select it in the dropdown. |

---

*Sample scene: `Samples/` (TestDummy prefab + Cube controller).*
