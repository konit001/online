# GizmosDrawer

A Unity editor-only toolkit for adding visual debug gizmos to GameObjects via MonoBehaviour components. All gizmos exist only in the Editor.

## Setup

Add any `Gizmo*` component to a GameObject and configure it in the Inspector. No additional setup is required.

## Namespace

All scripts are under the `DuzeraTools.GizmosDrawer` namespace. If you create custom gizmos or reference package types from your own scripts, add:

```csharp
using DuzeraTools.GizmosDrawer;
```

## Base Settings

Every gizmo inherits from `Gizmo` and exposes these shared fields:

| Field | Description |
|---|---|
| Draw | Toggle visibility |
| Selection Only | Only draw when the GameObject is selected |
| Color | Gizmo color |
| Size | Scale of the gizmo, optionally driven by a `FloatReference` |
| Referenced Size | Optional live float source for Size |
| Thickness | Line thickness |

## FloatReference

`FloatReference` lets you bind a gizmo value to a `float` field or property on a component in the scene. Set the target object, component type name, and member name in the Inspector. The gizmo reads the live value at draw time.

Several gizmos expose component-specific references, such as grid dimensions, FOV angle, rotation limits, trajectory length/height, and throw strength/gravity/duration.

---

## Gizmos Reference

### Primitives

| Component | Description |
|---|---|
| `GizmoArrowSingle` | Draws one flat arrow aligned to the transform forward/right orientation |
| `GizmoArrowMultiples` | Draws multiple arrows aligned to the transform forward/right orientation |
| `GizmoCapsule` | Draws a wireframe capsule with configurable radius, height, and axis direction |
| `GizmoCircle` | Draws a circle or arc around the object |
| `GizmoCube` | Draws a cube in wireframe or filled mode |
| `GizmoLine` | Draws a thin line through the transform |
| `GizmoPlane` | Draws a thin plane in wireframe or filled mode |
| `GizmoSphere` | Draws a sphere in wireframe or filled mode |
| `GizmoStar` | Draws a star in wireframe or filled mode |

### Vectors

| Component | Description |
|---|---|
| `GizmoAngle` | Draws the arc and angle between two target transforms |
| `GizmoDirection` | Draws an arrow from this transform toward a target transform, with optional Euler/Quaternion label |
| `GizmoDistance` | Draws a line between two transforms and labels the distance |
| `GizmoPath` | Draws a path through waypoint transforms, with linear or Catmull-Rom spline mode and distance labels |
| `GizmoSize` | Draws a bounds box and X/Y/Z size label from the object's renderers/colliders, optionally including children |

### Movement

| Component | Description |
|---|---|
| `GizmoThrow` | Draws a physics-style throw preview from transform forward using strength, gravity, and duration. Values can use `FloatReference`, and the trajectory can stop at colliders |
| `GizmoTrajectory` | Draws a designer-controlled forward arc using length and height. Values can use `FloatReference`, and the trajectory can stop at colliders |
| `GizmoVelocity` | Samples editor movement over time and draws a velocity direction arrow with an optional speed label |

### Locators

| Component | Description |
|---|---|
| `GizmoAxis` | Draws local X/Y/Z axes as color-coded arrows with optional labels |
| `GizmoLocator` | Draws a 3-axis crosshair locator at the transform position |
| `GizmoLooking` | Draws a disc and forward arrow to visualize facing direction |
| `GizmoTitle` | Renders a label and optional sprite icon above the transform in the Scene view |

### Colliders

| Component | Description |
|---|---|
| `GizmoFOV` | Draws a field-of-view sector on the XZ plane, with optional fill and collider clipping |
| `GizmoRayCast` | Casts a ray along transform forward and visualizes the ray, hit point, normal, and dotted remainder line |

### Grids

| Component | Description |
|---|---|
| `Gizmo2DGrid` | Draws a flat width/depth grid with alignment, optional points, and cell index labels |
| `Gizmo3DGrid` | Draws a volumetric width/depth/height grid with alignment, optional points, and cell index labels |

### Rigging

| Component | Description |
|---|---|
| `GizmoBones` | Draws transform hierarchy bones with optional joint spheres |
| `GizmoRotation` | Draws local rotation limits as an arc, with optional referenced min/max angles |

---

## Folder Structure

```text
GizmosDrawer/
|-- README.md
|-- Scripts/
    |-- Base/
    |   |-- Gizmo.cs
    |   |-- GizmoUtils.cs
    |-- Editor/
    |   |-- FloatReferenceDrawer.cs
    |   |-- GizmoEditor.cs
    |   |-- MinMaxSliderDrawer.cs
    |-- Utils/
    |   |-- FloatReference.cs
    |   |-- MinMaxSliderAttribute.cs
    |-- Gizmos/
        |-- Colliders/
        |-- Grids/
        |-- Locators/
        |-- Movement/
        |-- Primitives/
        |-- Rigging/
        |-- Vectors/
```
