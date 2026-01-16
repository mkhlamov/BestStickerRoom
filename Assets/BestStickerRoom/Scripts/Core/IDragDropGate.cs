using UniRx;

namespace BestStickerRoom.Core
{
    public interface IDragDropGate
    {
        public ReactiveProperty<bool> DragAllowed { get; }
    }
}
