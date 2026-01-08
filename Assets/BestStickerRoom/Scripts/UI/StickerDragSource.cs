using UnityEngine;
using BestStickerRoom.Core;

namespace BestStickerRoom.UI
{
    public class StickerDragSource : MonoBehaviour, IDragSource
    {
        [SerializeField] private Texture2D stickerTexture;
        [SerializeField] private Sprite stickerSprite;

        public object GetDragData()
        {
            return new StickerData
            {
                Texture = stickerTexture,
                Sprite = stickerSprite
            };
        }

        public bool CanDrag()
        {
            return stickerTexture != null || stickerSprite != null;
        }
    }

    public class StickerData
    {
        public Texture2D Texture { get; set; }
        public Sprite Sprite { get; set; }
    }
}
