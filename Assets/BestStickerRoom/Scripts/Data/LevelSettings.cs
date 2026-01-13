using UnityEngine;

namespace BestStickerRoom.Data
{
    [CreateAssetMenu(fileName = "LevelSettings", menuName = "BestStickerRoom/Level Settings")]
    public class LevelSettings : ScriptableObject
    {
        [SerializeField] private GameObject roomPrefab;
        [SerializeField] private GameObject stickerPrefab;
        [SerializeField] private float stickerOffsetFromSurface = 0.01f;
        [SerializeField] private Vector2 stickerSize = new Vector2(0.3f, 0.3f);
        [SerializeField] private StickerPack stickerPack;

        public GameObject RoomPrefab => roomPrefab;
        public GameObject StickerPrefab => stickerPrefab;
        public float StickerOffsetFromSurface => stickerOffsetFromSurface;
        public Vector2 StickerSize => stickerSize;
        public StickerPack StickerPack => stickerPack;
    }
}
