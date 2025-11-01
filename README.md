# Unity Essentials

This module is part of the Unity Essentials ecosystem and follows the same lightweight, editor-first approach.
Unity Essentials is a lightweight, modular set of editor utilities and helpers that streamline Unity development. It focuses on clean, dependency-free tools that work well together.

All utilities are under the `UnityEssentials` namespace.

```csharp
using UnityEssentials;
```

## Installation

Install the Unity Essentials entry package via Unity's Package Manager, then install modules from the Tools menu.

- Add the entry package (via Git URL)
    - Window → Package Manager
    - "+" → "Add package from git URL…"
    - Paste: `https://github.com/CanTalat-Yakan/UnityEssentials.git`

- Install or update Unity Essentials packages
    - Tools → Install & Update UnityEssentials
    - Install all or select individual modules; run again anytime to update

---

# UI Toolkit Marquee Label

> Quick overview: A custom UI Toolkit `<MarqueeLabel>` that automatically scrolls overflowing text horizontally. Drop it in UXML, set `scroll-speed` and `pause-time`, and it will wrap itself in a hidden‑overflow container and animate only when content exceeds the available width.

A self‑contained marquee control for UI Toolkit. When its text is wider than its layout width, it creates a local container with `overflow: hidden`, positions itself `absolute`, and scrolls the text from left to right with short pauses. If the text fits, it doesn’t animate and stays still.

![screenshot](Documentation/Screenshot.png)

## Features
- Plug‑and‑play UXML element
  - Use `<MarqueeLabel>` just like a normal `<Label>` (inherits `UnityEngine.UIElements.Label`)
- Auto‑wrap and clip
  - On first geometry pass, it creates a sibling container and moves itself inside, with `overflow: hidden`
- Scrolls only when needed
  - If the text width is greater than the container width, scrolling activates; otherwise it remains static
- Simple tuning
  - UXML attributes: `scroll-speed` (units ≈ font size per second) and `pause-time` (seconds at edges)
- Sensible behavior
  - Pauses at the end, then resets and restarts periodically; respects absolute positioning and copies position offsets from the original

## Requirements
- Unity 6000.0+
- UI Toolkit (UXML/USS)

## Usage

### UXML
```xml
<!-- In a UXML file (UI Builder will recognize custom controls with [UxmlElement]) -->
<ui:UXML xmlns:ui="UnityEngine.UIElements" xmlns:uie="UnityEditor.UIElements">
  <MarqueeLabel text="This text will scroll when it is longer than the available space." scroll-speed="10" pause-time="1" />
</ui:UXML>
```

- `scroll-speed`: float (default 10). Effective pixels/sec scale with font size
- `pause-time`: float seconds (default 1). Pause at start/end and on reset

### C# (runtime)
```csharp
using UnityEngine;
using UnityEngine.UIElements;

public class MarqueeExample : MonoBehaviour
{
    public UIDocument document;

    void OnEnable()
    {
        var root = document.rootVisualElement;
        var marquee = new MarqueeLabel
        {
            text = "Breaking news: UI Toolkit marquee label scrolling example…",
            ScrollSpeed = 12f,
            PauseTime = 0.75f
        };

        // Size and style however you like; the control will manage its own internal wrapper
        marquee.style.width = 300;
        marquee.style.unityTextAlign = TextAnchor.MiddleLeft;

        root.Add(marquee);
    }
}
```

## How It Works
- Initialization
  - On first `GeometryChangedEvent`, the label schedules a one‑time setup: creates a container `VisualElement` with `overflow: hidden`, copies positional styles (for absolute) and width/height, inserts the container in place of the label, then adds the label inside and sets the label to `position: absolute; width: auto`
- Scrolling loop
  - Subscribes to container geometry changes; when text width > container width it starts scrolling
  - A scheduled `Update` runs roughly every 16 ms (~60 FPS) and adjusts `style.left` by `ScrollSpeed * fontSize * deltaTime`
  - When reaching the end, it pauses for `PauseTime`, resets to start after a short interval, and continues
- Static case
  - If content fits, scrolling remains disabled and `left` stays at 0

## Notes and Limitations
- Scroll speed unit: multiplied by the current font size; changing fonts affects effective speed
- Restart cadence uses an internal 3‑second interval for resets; currently not exposed as an attribute
- Multiple marquees are fine; each runs its own lightweight 60 FPS scheduler. Very large numbers may impact UI performance
- The control copies absolute positioning from the original; relative/flow layout should “just work” inside its wrapper
- The label sets its own `position: absolute` within the wrapper; avoid overriding that unless you know what you’re doing

## Files in This Package
- `Runtime/MarqueeLabel.cs` – The custom element implementation with UXML support
- `Runtime/UnityEssentials.UIToolkitMarqueeLabel.asmdef` – Runtime assembly definition
- `package.json`, `LICENSE.md`

## Tags
unity, ui toolkit, uitoolkit, marquee, label, text, scrolling, overflow, uxml
