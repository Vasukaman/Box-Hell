# Conveyor Belt System for Unity

**Author:** Faktory Studios  
**Version:** 1.0  
**Compatibility:** Unity 2020.3+ (URP & Built-In)

---

## 🚀 Features

- Fully parametric mesh: width, height, length, rounded ends
- Scrolls UVs for animation
- Moves rigidbodies across the belt
- Audio feedback on start/stop
- Fully polished custom inspector with tooltips and logos
- Scene gizmos for trigger zones and direction

---

## 🎮 Quickstart

1. **Add the Component**
   - Attach `ConveyorBelt.cs` to an empty GameObject 
   OR
   - Create a conveyor from the 'GameObject->3D Object->Conveyor Belt' drop down.

2. **Assign the Material**
   - Use any material that supports `_BaseMap` UV offset.

3. **Adjust Shape**
   - Set Width (Z), Height (Y), and Length (X) to define size.
   - Increase Corner Segments for smoother ends.

4. **Set Extrusion Detail**
   - Use Segments to control mesh resolution.

5. **Enable UV Motion**
   - Set UV Scale U/V and Scroll Speed.
   - Use Scroll Curve to animate speed changes.

6. **Configure Physics**
   - Toggle 'Move Objects in Trigger' if desired.
   - Objects with rigidbodies will move with the belt.

7. **Audio Feedback**
   - Optionally assign Start, Stop, and Running audio clips.

8. **Visual Debugging**
   - Enable 'Show Gizmos' to visualize conveyor direction and trigger zone.

9. **Control at Runtime**
   - Use Inspector buttons to Start ⏵ or Stop ⏸ the belt manually.
```

---

## 📁 Folder Structure

```
Assets/
├── Scripts/
│   └── ConveyorBelt.cs
├── Editor/
│   └── ConveyorBeltEditor.cs
│   └── Resources/
│       └── ConveyorBeltLogo.png
```

---

## 📄 License

© 2025 Faktory Studios. All rights reserved.

---

## 🛠️ Support

Email: support@faktorystudios.com
Discord: https://discord.gg/mdZ2wCCA9f
