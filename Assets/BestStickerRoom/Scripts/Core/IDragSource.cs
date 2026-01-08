using UnityEngine;

namespace BestStickerRoom.Core
{
    public interface IDragSource
    {
        object GetDragData();
        bool CanDrag();
    }
}
