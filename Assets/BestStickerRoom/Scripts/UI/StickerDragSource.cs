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
        public Texture2D AtlasTexture => Asset?.AtlasTexture;
        public Sprite Sprite => Asset?.Sprite;
        public Vector2 UVOffset => Asset?.UVOffset ?? Vector2.zero;
        public Vector2 UVScale => Asset?.UVScale ?? Vector2.one;

        public StickerData(StickerAsset asset)
        {
            Asset = asset;
        }
    }
}
