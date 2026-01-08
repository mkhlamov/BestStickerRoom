// 1/8/2026 AI-Tag
// This was created with the help of Assistant, a Unity Artificial Intelligence product.

using UnityEngine;

namespace BestStickerRoom.Animation
{
    public class AnimatedSticker : MonoBehaviour
    {
        public Sprite[] animationFrames; // Array of sprites for animation
        public float frameRate = 0.1f; // Time between frames

        private MeshRenderer meshRenderer;
        private int currentFrame;
        private float timer;

        void Start()
        {
            meshRenderer = GetComponent<MeshRenderer>();
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
            if (meshRenderer != null && animationFrames[currentFrame] != null)
            {
                meshRenderer.material.mainTexture = animationFrames[currentFrame].texture;
            }
        }
    }
}
