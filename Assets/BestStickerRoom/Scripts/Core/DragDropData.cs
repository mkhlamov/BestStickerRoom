using UnityEngine;

namespace BestStickerRoom.Core
{
    public class DragDropData
    {
        public object Data { get; set; }
        public Vector2 StartScreenPosition { get; set; }
        public Vector2 CurrentScreenPosition { get; set; }
        public GameObject SourceGameObject { get; set; }
        public IDragSource Source { get; set; }
    }
}
