using System;
using UnityEngine;

namespace BestStickerRoom.Room
{
    public interface IWallDetector
    {
        event Action<WallHitResult> OnWallDetected;
        WallHitResult DetectWallFromScreenPosition(Vector2 screenPosition);
        WallHitResult DetectWallFromWorldRay(Ray worldRay);
        WallHitResult DetectWallFromTouchOrMouse();
        bool IsValidWallHit(WallHitResult hitResult);
    }
}
