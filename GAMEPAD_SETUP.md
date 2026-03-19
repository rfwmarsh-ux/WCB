# Gamepad Setup Instructions

## Unity Input Manager Configuration

To use gamepad controls, you need to set up the Input Manager in Unity. Follow these steps:

### 1. Open Unity Input Manager
Go to **Edit > Project Settings > Input Manager**

### 2. Add These Axes (or modify existing ones)

#### Player 1 Controls
| Axis Name | Positive Button | Alt Positive | Gravity | Dead | Sensitivity |
|-----------|----------------|--------------|---------|------|-------------|
| Horizontal | a, d | left, right | 3 | 0.001 | 3 |
| Vertical | w, s | up, down | 3 | 0.001 | 3 |
| Fire1 | joystick button 0 | mouse 0 | 1000 | 0.001 | 1000 |
| Fire2 | joystick button 1 | - | 1000 | 0.001 | 1000 |
| Fire3 | joystick button 3 (left trigger) | left shift | 1000 | 0.001 | 1000 |
| Jump | joystick button 2 | space | 1000 | 0.001 | 1000 |
| Mouse X | - | - | 0.1 | 0 | 0.1 |
| Mouse Y | - | - | 0.1 | 0 | 0.1 |

#### Add New Axes (Click + button)

**P1_Kick:**
- Positive Button: `joystick button 4`
- Gravity: 1000
- Dead: 0.001
- Sensitivity: 1000

**P1_Grenade:**
- Positive Button: `joystick button 5`
- Gravity: 1000
- Dead: 0.001
- Sensitivity: 1000

**P1_Reload:**
- Positive Button: `joystick button 3`
- Alt Positive: `r`
- Gravity: 1000
- Dead: 0.001
- Sensitivity: 1000

**P1_Steal:**
- Positive Button: `joystick button 1`
- Alt Positive: `f`
- Gravity: 1000
- Dead: 0.001
- Sensitivity: 1000

**P1_Heal:**
- Positive Button: `joystick button 6`
- Alt Positive: `h`
- Gravity: 1000
- Dead: 0.001
- Sensitivity: 1000

**Joy1Axis3 (Right Stick X):**
- Axis: 3rd Axis (Joysticks)
- Joy Num: Joystick 1
- Gravity: 0.1
- Dead: 0.2
- Sensitivity: 3

**Joy1Axis4 (Right Stick Y):**
- Axis: 4th Axis (Joysticks)
- Joy Num: Joystick 1
- Gravity: 0.1
- Dead: 0.2
- Sensitivity: 3

#### Player 2 Controls

**P2_Horizontal:**
- Positive Button: `a`, `d`
- Alt Positive: `left`, `right`
- Axis: 1st Axis (Joysticks)
- Joy Num: Joystick 2
- Gravity: 3
- Dead: 0.001
- Sensitivity: 3

**P2_Vertical:**
- Positive Button: `w`, `s`
- Alt Positive: `up`, `down`
- Axis: 2nd Axis (Joysticks)
- Joy Num: Joystick 2
- Gravity: 3
- Dead: 0.001
- Sensitivity: 3

**P2_Fire:**
- Positive Button: `joystick button 0`
- Gravity: 1000
- Dead: 0.001

**P2_Jump:**
- Positive Button: `joystick button 2`
- Alt Positive: `numlock1`
- Gravity: 1000

**P2_Kick:**
- Positive Button: `joystick button 4`
- Gravity: 1000

**P2_Grenade:**
- Positive Button: `joystick button 5`
- Gravity: 1000

---

## Standard Gamepad Button Mapping (Xbox/PlayStation Style)

### Player 1
| Button | Action |
|--------|--------|
| Left Stick | Move |
| Right Stick | Aim |
| A / Cross (Button 0) | Fire |
| B / Circle (Button 1) | Steal Vehicle |
| X / Square (Button 2) | Melee |
| Y / Triangle (Button 3) | Kick / Interact |
| LB (Button 4) | Grenade |
| RB (Button 5) | Reload |
| LT (Button 6) | Run (hold) |
| RT (Button 7) | - |
| Start | Pause |

### Player 2
| Button | Action |
|--------|--------|
| Left Stick | Move |
| Right Stick | Aim |
| A / Cross | Fire |
| B / Circle | Steal Vehicle |
| X / Square | Melee |
| Y / Triangle | Kick / Interact |
| LB | Grenade |
| RB | Reload |
| LT | Run (hold) |

---

## In-Game Settings

Press **Settings** from the main menu to toggle between:
- **Controls: Keyboard** - Default keyboard/mouse
- **Controls: Gamepad** - Gamepad controls

Toggle with Enter key in Settings menu.