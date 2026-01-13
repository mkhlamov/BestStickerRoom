using UnityEngine;

namespace BestStickerRoom.Room
{
    public class RoomHitValidator : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer spriteRenderer;
        
        private Texture2D hitMask;
        private Sprite sprite;
        private bool isInitialized;

        private void Awake()
        {
            Initialize();
        }

        private void Initialize()
        {
            if (isInitialized) return;

            if (spriteRenderer == null)
            {
                spriteRenderer = GetComponentInChildren<SpriteRenderer>();
            }

            if (spriteRenderer == null || spriteRenderer.sprite == null)
            {
                Debug.LogError($"RoomHitValidator: No SpriteRenderer or Sprite found on {gameObject.name}");
                return;
            }

            sprite = spriteRenderer.sprite;
            PreCalculateHitMask();
            isInitialized = true;
        }

        private void PreCalculateHitMask()
        {
            var sourceTexture = sprite.texture;
            
            if (!sourceTexture.isReadable)
            {
                Debug.LogError($"RoomHitValidator: Texture '{sourceTexture.name}' must be readable. Enable Read/Write in import settings.");
                return;
            }

            var spriteRect = sprite.rect;
            var width = Mathf.RoundToInt(spriteRect.width);
            var height = Mathf.RoundToInt(spriteRect.height);

            hitMask = new Texture2D(width, height, TextureFormat.R8, false);
            hitMask.filterMode = FilterMode.Point;

            for (var y = 0; y < height; y++)
            {
                for (var x = 0; x < width; x++)
                {
                    var textureX = Mathf.FloorToInt(spriteRect.x + x);
                    var textureY = Mathf.FloorToInt(spriteRect.y + y);
                    var pixel = sourceTexture.GetPixel(textureX, textureY);
                    
                    var isOpaque = pixel.a > 0f;
                    hitMask.SetPixel(x, y, isOpaque ? Color.white : Color.clear);
                }
            }

            hitMask.Apply();
        }

        public bool IsValidHitPoint(Vector3 worldPoint)
        {
            if (!isInitialized)
            {
                Initialize();
            }

            if (hitMask == null || sprite == null)
            {
                return false;
            }

            var localPoint = transform.InverseTransformPoint(worldPoint);
            
            var pixelsPerUnit = sprite.pixelsPerUnit;
            var pivot = sprite.pivot;
            
            var pixelX = (localPoint.x * pixelsPerUnit) + pivot.x;
            var pixelY = (localPoint.y * pixelsPerUnit) + pivot.y;
            
            var maskX = Mathf.FloorToInt(pixelX);
            var maskY = Mathf.FloorToInt(pixelY);
            
            if (maskX < 0 || maskX >= hitMask.width || maskY < 0 || maskY >= hitMask.height)
            {
                return false;
            }
            
            var maskPixel = hitMask.GetPixel(maskX, maskY);
            return maskPixel.r > 0.5f;
        }

        private void OnDestroy()
        {
            if (hitMask != null)
            {
                Destroy(hitMask);
            }
        }

#if UNITY_EDITOR
        [ContextMenu("Visualize Hit Mask")]
        private void VisualizeHitMask()
        {
            if (!isInitialized)
            {
                Initialize();
            }

            if (hitMask != null)
            {
                var path = $"Assets/Debug_HitMask_{gameObject.name}.png";
                System.IO.File.WriteAllBytes(path, hitMask.EncodeToPNG());
                UnityEditor.AssetDatabase.Refresh();
                Debug.Log($"Hit mask saved to {path}");
            }
        }
#endif
    }
}
