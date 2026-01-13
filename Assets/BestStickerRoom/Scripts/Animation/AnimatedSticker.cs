// 1/8/2026 AI-Tag
// This was created with the help of Assistant, a Unity Artificial Intelligence product.

using UnityEngine;

namespace BestStickerRoom.Animation
{
    public class AnimatedSticker : MonoBehaviour
    {
        public Sprite[] animationFrames;
        public float frameRate = 0.1f;

        private SpriteRenderer spriteRenderer;
        private int currentFrame;
        private float timer;

        void Start()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            if (animationFrames.Length > 0)
            {
                UpdateFrame();
            }
        }

        void Update()
        {
            if (animationFrames.Length > 1)
            {
                timer += Time.deltaTime;
                if (timer >= frameRate)
                {
                    timer = 0f;
                    currentFrame = (currentFrame + 1) % animationFrames.Length;
                    UpdateFrame();
                }
            }
        }

        void UpdateFrame()
        {
            if (spriteRenderer != null && animationFrames[currentFrame] != null)
            {
                spriteRenderer.sprite = animationFrames[currentFrame];
            }
        }
    }
}
