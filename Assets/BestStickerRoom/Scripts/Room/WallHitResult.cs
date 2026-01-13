using UnityEngine;

namespace BestStickerRoom.Room
{
    public struct WallHitResult
    {
        public bool IsValid;
        public Vector3 HitPoint;
        public GameObject WallObject;
        public Transform WallTransform;
        public float Distance;

        public static WallHitResult Invalid => new WallHitResult
        {
            IsValid = false
        };

        public static WallHitResult Create(RaycastHit2D hit, RoomHitValidator validator)
        {
            if (hit.collider == null)
            {
                return Invalid;
            }

            if (validator != null && !validator.IsValidHitPoint(hit.point))
            {
                return Invalid;
            }

            return new WallHitResult
            {
                IsValid = true,
                HitPoint = hit.point,
                WallObject = hit.collider.gameObject,
                WallTransform = hit.collider.transform,
                Distance = hit.distance
            };
        }

        public static WallHitResult Create(Vector3 hitPoint, GameObject wallObject, RoomHitValidator validator)
        {
            if (wallObject == null)
            {
                return Invalid;
            }

            if (validator != null && !validator.IsValidHitPoint(hitPoint))
            {
                return Invalid;
            }

            return new WallHitResult
            {
                IsValid = true,
                HitPoint = hitPoint,
                WallObject = wallObject,
                WallTransform = wallObject.transform,
                Distance = 0f
            };
        }
    }
}

