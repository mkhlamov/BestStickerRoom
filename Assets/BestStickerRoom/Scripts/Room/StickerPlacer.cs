using System;
using UnityEngine;
using BestStickerRoom.Core;
using BestStickerRoom.Data;
using BestStickerRoom.UI;
using Zenject;

namespace BestStickerRoom.Room
{
    public class StickerPlacer : MonoBehaviour
    {
        private LevelSettings levelSettings;
        private DragDropHandler dragDropHandler;
        private Transform stickerParent;

        public event Action<GameObject> OnStickerPlaced;

        [Inject]
        private void Construct(DragDropHandler dragDrop, LevelSettings settings)
        {
            dragDropHandler = dragDrop;
            levelSettings = settings;
        }

        private void Awake()
        {
            if (levelSettings == null)
            {
                Debug.LogError("StickerPlacer: LevelSettings is not assigned!");
            }
        }

        private void OnEnable()
        {
            if (dragDropHandler != null)
            {
                dragDropHandler.OnDragDropped += HandleDragDropped;
            }
        }

        private void OnDisable()
        {
            if (dragDropHandler != null)
            {
                dragDropHandler.OnDragDropped -= HandleDragDropped;
            }
        }

        private void HandleDragDropped(DragDropData dragData, WallHitResult wallHit)
        {
            if (levelSettings == null || levelSettings.StickerPrefab == null)
            {
                Debug.LogError("StickerPlacer: LevelSettings or StickerPrefab is null!");
                return;
            }

            if (!wallHit.IsValid)
            {
                return;
            }

            var stickerInstance = InstantiateSticker(wallHit);
            if (stickerInstance != null)
            {
                ApplyStickerData(stickerInstance, dragData);
                OnStickerPlaced?.Invoke(stickerInstance);
            }
        }

        private GameObject InstantiateSticker(WallHitResult wallHit)
        {
            var stickerPrefab = levelSettings.StickerPrefab;
            var offset = wallHit.HitNormal * levelSettings.StickerOffsetFromSurface;
            var position = wallHit.HitPoint + offset;

            var rotation = Quaternion.LookRotation(-wallHit.HitNormal, Vector3.up);

            var stickerInstance = Instantiate(stickerPrefab, position, rotation);

            if (stickerParent == null)
            {
                stickerParent = new GameObject("Stickers").transform;
            }

            stickerInstance.transform.SetParent(stickerParent);

            var stickerTransform = stickerInstance.transform;
            stickerTransform.localScale = new Vector3(
                levelSettings.StickerSize.x,
                levelSettings.StickerSize.y,
                1f
            );

            return stickerInstance;
        }

        private void ApplyStickerData(GameObject stickerInstance, DragDropData dragData)
        {
            if (dragData?.Data is StickerData stickerData)
            {
                var meshRenderer = stickerInstance.GetComponentInChildren<MeshRenderer>();
                if (meshRenderer != null)
                {
                    var material = meshRenderer.material;
                    if (material != null)
                    {
                        if (stickerData.Texture != null)
                        {
                            material.mainTexture = stickerData.Texture;
                        }
                        else if (stickerData.Sprite != null)
                        {
                            material.mainTexture = stickerData.Sprite.texture;
                        }
                    }
                }
            }
        }

    }
}
