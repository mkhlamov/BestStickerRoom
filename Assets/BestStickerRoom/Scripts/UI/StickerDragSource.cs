using UnityEngine;
using BestStickerRoom.Core;
using BestStickerRoom.Data;

namespace BestStickerRoom.UI
{
    public class StickerDragSource : MonoBehaviour, IDragSource
    {
        [SerializeField] private StickerAsset stickerAsset;

        public object GetDragData()
        {
            if (stickerAsset == null) return null;
            return new StickerData(stickerAsset);
        }

        public bool CanDrag()
        {
            return stickerAsset != null && stickerAsset.Sprite != null;
        }
    }

    public class StickerData
    {
        public StickerAsset Asset { get; }
        public Sprite Sprite => Asset?.Sprite;

        public StickerData(StickerAsset asset)
        {
            Asset = asset;
        }
    }
}
