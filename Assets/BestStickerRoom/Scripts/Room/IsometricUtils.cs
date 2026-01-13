using UnityEngine;

namespace BestStickerRoom.Room
{
    public static class IsometricUtils
    {
        private const float DEFAULT_TILE_WIDTH = 1f;
        private const float DEFAULT_TILE_HEIGHT = 0.5f;
        
        public static Vector2 CartesianToIsometric(Vector2 cartesian)
        {
            var isometric = new Vector2
            {
                x = (cartesian.x - cartesian.y) * DEFAULT_TILE_WIDTH,
                y = (cartesian.x + cartesian.y) * DEFAULT_TILE_HEIGHT
            };
            return isometric;
        }
        
        public static Vector2 IsometricToCartesian(Vector2 isometric)
        {
            var cartesian = new Vector2
            {
                x = (isometric.x / DEFAULT_TILE_WIDTH + isometric.y / DEFAULT_TILE_HEIGHT) / 2f,
                y = (isometric.y / DEFAULT_TILE_HEIGHT - isometric.x / DEFAULT_TILE_WIDTH) / 2f
            };
            return cartesian;
        }
        
        public static Vector3 ScreenToIsometricWorld(Vector2 screenPosition, Camera camera, float zPlane = 0f)
        {
            if (camera == null)
            {
                Debug.LogError("IsometricUtils: Camera is null");
                return Vector3.zero;
            }
            
            if (!camera.orthographic)
            {
                Debug.LogWarning("IsometricUtils: Camera should be orthographic for proper isometric projection");
            }
            
            var worldPoint = camera.ScreenToWorldPoint(new Vector3(screenPosition.x, screenPosition.y, camera.nearClipPlane + zPlane));
            worldPoint.z = zPlane;
            
            return worldPoint;
        }
        
        public static Vector2 IsometricWorldToScreen(Vector3 worldPosition, Camera camera)
        {
            if (camera == null)
            {
                Debug.LogError("IsometricUtils: Camera is null");
                return Vector2.zero;
            }
            
            var screenPoint = camera.WorldToScreenPoint(worldPosition);
            return new Vector2(screenPoint.x, screenPoint.y);
        }
        
        public static int CalculateSortingOrder(Vector3 worldPosition, int baseSortingOrder = 0, float sortingMultiplier = 100f)
        {
            return baseSortingOrder - Mathf.RoundToInt(worldPosition.y * sortingMultiplier);
        }
        
        public static void UpdateSortingOrder(SpriteRenderer spriteRenderer, Vector3 worldPosition, int baseSortingOrder = 0, float sortingMultiplier = 100f)
        {
            if (spriteRenderer == null) return;
            spriteRenderer.sortingOrder = CalculateSortingOrder(worldPosition, baseSortingOrder, sortingMultiplier);
        }
    }
}
