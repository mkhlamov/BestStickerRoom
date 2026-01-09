using UnityEngine;
using UnityEngine.UI;
using BestStickerRoom.Core;
using BestStickerRoom.Data;

namespace BestStickerRoom.UI
{
    public class StickerItemUI : MonoBehaviour, IDragSource
    {
        [SerializeField] private Image stickerImage;

        private StickerAsset stickerAsset;

        private void Awake()
        {
            if (stickerImage != null && !stickerImage.raycastTarget)
            {
                stickerImage.raycastTarget = true;
            }
        }

        public void Initialize(StickerAsset asset)
        {
            stickerAsset = asset;
            UpdateImage();
        }

        public object GetDragData()
        {
            if (stickerAsset == null) return null;
            return new StickerData(stickerAsset);
        }

        public bool CanDrag()
        {
            return stickerAsset != null && stickerAsset.Sprite != null;
        }

        private void UpdateImage()
        {
            if (stickerImage == null)
            {
                stickerImage = GetComponentInChildren<Image>();
            }

            if (stickerImage != null && stickerAsset != null && stickerAsset.Sprite != null)
            {
                stickerImage.sprite = stickerAsset.Sprite;
            }
        }
    }
}
