using UnityEngine;

namespace BestStickerRoom.Room
{
    public class Room : MonoBehaviour
    {
        [SerializeField] private Collider2D roomCollider;

        public Collider2D Collider => roomCollider;

        private void Awake()
        {
            if (roomCollider == null)
            {
                roomCollider = GetComponent<Collider2D>();
                if (roomCollider == null)
                {
                    Debug.LogError("Room: Collider2D is not assigned!");
                }
            }
        }
    }
}
