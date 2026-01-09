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

        public Texture2D AtlasTexture
        {
            get
            {
                if (sprite != null && sprite.texture != null)
                {
                    return sprite.texture;
                }
                return null;
            }
        }

        public Vector2 UVOffset
        {
            get
            {
                if (sprite == null) return Vector2.zero;

                var rect = sprite.textureRect;
                var texture = sprite.texture;
                if (texture == null) return Vector2.zero;

                return new Vector2(rect.x / texture.width, rect.y / texture.height);
            }
        }

        public Vector2 UVScale
        {
            get
            {
                if (sprite == null) return Vector2.one;

                var rect = sprite.textureRect;
                var texture = sprite.texture;
                if (texture == null) return Vector2.one;

                return new Vector2(rect.width / texture.width, rect.height / texture.height);
            }
        }

        public void SetPack(StickerPack pack)
        {
            this.pack = pack;
        }
    }
}
