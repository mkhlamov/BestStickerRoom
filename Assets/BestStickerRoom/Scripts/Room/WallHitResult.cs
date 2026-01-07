using UnityEngine;

namespace BestStickerRoom.Room
{
    public struct WallHitResult
    {
        public bool IsValid;
        public Vector3 HitPoint;
        public Vector3 HitNormal;
        public GameObject WallObject;
        public Transform WallTransform;
        public Collider WallCollider;

        public static WallHitResult Invalid => new WallHitResult
        {
            IsValid = false
        };

        public static WallHitResult Create(RaycastHit hit)
        {
            return new WallHitResult
            {
                IsValid = true,
                HitPoint = hit.point,
                HitNormal = hit.normal,
                WallObject = hit.collider.gameObject,
                WallTransform = hit.collider.transform,
                WallCollider = hit.collider
            };
        }
    }
}

