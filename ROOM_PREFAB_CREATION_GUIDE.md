# Room2DIsometric Prefab Creation Guide

## Overview
Create a 2D sprite-based isometric room prefab to replace the current 3D Room3D prefab.

## Step-by-Step Instructions

### 1. Create Root GameObject
1. In Unity Hierarchy, right-click → Create Empty
2. Name it: `Room2DIsometric`
3. Set Transform Position to `(0, 0, 0)`
4. Set Transform Rotation to `(0, 0, 0)`
5. Set Transform Scale to `(1, 1, 1)`

### 2. Add Room Sprite
1. Right-click `Room2DIsometric` → 2D Object → Sprite
2. Name it: `RoomSprite`
3. In Inspector:
   - **Transform Position**: `(0, 0, 0)` (adjust as needed for your isometric view)
   - **Sprite Renderer**:
     - Sprite: Assign your isometric room sprite texture
     - Sorting Layer: Create/Select "Room" or "Default"
     - Order in Layer: `0`
   - **Add Component** → `Polygon Collider 2D`
     - Auto-generates shape from sprite's opaque pixels
     - Click "Edit Collider" to manually adjust points if needed

### 3. Configure Collider for Hit Detection
The Polygon Collider 2D auto-generates its shape from the sprite's opaque pixels, providing pixel-perfect collision detection.

- Click "Edit Collider" to manually adjust collision points if needed
- Enable "Is Trigger" if you don't want physics interactions
- The collider shape = clickable area (transparent pixels are automatically excluded)

### 4. Set Tag
1. Select `RoomSprite` GameObject
2. In Inspector, click Tag dropdown → Add Tag
3. Create new tag: `Room`
4. Select `RoomSprite` again and set Tag to `Room`

### 5. Create Prefab
1. Drag the `Room2DIsometric` GameObject from Hierarchy into:
   `Assets/BestStickerRoom/Prefabs/`
2. Unity will create `Room2DIsometric.prefab`
3. You can now delete the instance from the Hierarchy (we'll add it back via the scene later)

### 6. Sprite Texture Import Settings
Your room sprite texture should have these import settings:

1. Select your room sprite texture in Project window
2. In Inspector:
   - **Texture Type**: `Sprite (2D and UI)`
   - **Sprite Mode**: `Single`
   - **Pixels Per Unit**: `100` (or your preferred value)
   - **Filter Mode**: `Point` (for pixel-perfect) or `Bilinear` (for smooth)
   - **Compression**: `None` or `High Quality`
   - Click **Apply**

### 7. Sorting Layers Setup (Optional but Recommended)
Create proper sorting layers for depth management:

1. Edit → Project Settings → Tags and Layers
2. Add Sorting Layers (in order):
   - `Background` (Order: 0)
   - `Room` (Order: 1)
   - `Stickers` (Order: 2)
   - `UI` (Order: 3)

3. Set RoomSprite's Sorting Layer to `Room`

## Prefab Structure Summary

```
Room2DIsometric (GameObject)
└── RoomSprite (GameObject)
    ├── Transform (Position: 0, 0, 0)
    ├── Sprite Renderer
    │   ├── Sprite: [Your Isometric Room Sprite]
    │   ├── Sorting Layer: Room
    │   └── Order in Layer: 0
    ├── Polygon Collider 2D
    │   └── Auto-generated from sprite shape
    └── Tag: Room
```

## Troubleshooting

### Clicks not detecting the room
- **Solution**: Verify the GameObject has Tag set to "Room"

### Clicks detecting in transparent areas
- **Solution**: Regenerate Polygon Collider 2D or manually edit collision points

### Collider is too small/large
- **Solution**: Regenerate Polygon Collider 2D or manually edit collision points

## Next Steps
After creating the prefab:
1. Update `TestLevelSettings.asset` to reference the new `Room2DIsometric` prefab
2. Add the prefab to `GameplayScene`
