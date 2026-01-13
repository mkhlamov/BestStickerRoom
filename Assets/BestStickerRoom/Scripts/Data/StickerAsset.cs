using UnityEngine;

namespace BestStickerRoom.Data
{
    public enum AllowedPosition
    {
        Anywhere,
        Wall,
        Floor,
        Ceiling,
        VerticalOnly,
        HorizontalOnly
    }

    [CreateAssetMenu(fileName = "StickerAsset", menuName = "BestStickerRoom/Sticker Asset")]
    public class StickerAsset : ScriptableObject
    {
        [SerializeField] private Sprite sprite;
        [SerializeField] private string stickerName;
        [SerializeField] private AllowedPosition allowedPosition = AllowedPosition.Anywhere;
        [SerializeField] private StickerPack pack;

        public Sprite Sprite => sprite;
        public string StickerName => stickerName;
        public AllowedPosition AllowedPosition => allowedPosition;
        public StickerPack Pack => pack;

        public void SetPack(StickerPack pack)
        {
            this.pack = pack;
        }
    }
}
