using System.Collections.Generic;
using UnityEngine;

namespace BestStickerRoom.Data
{
    [CreateAssetMenu(fileName = "StickerPack", menuName = "BestStickerRoom/Sticker Pack")]
    public class StickerPack : ScriptableObject
    {
        [SerializeField] private string packName;
        [SerializeField] private string description;
        [SerializeField] private Sprite previewImage;
        [SerializeField] private float price;
        [SerializeField] private List<StickerAsset> stickers = new List<StickerAsset>();

        public string PackName => packName;
        public string Description => description;
        public Sprite PreviewImage => previewImage;
        public float Price => price;
        public IReadOnlyList<StickerAsset> Stickers => stickers;

        public void AddSticker(StickerAsset sticker)
        {
            if (sticker != null && !stickers.Contains(sticker))
            {
                stickers.Add(sticker);
                sticker.SetPack(this);
            }
        }

        public void RemoveSticker(StickerAsset sticker)
        {
            if (stickers.Remove(sticker))
            {
                sticker.SetPack(null);
            }
        }
    }
}
