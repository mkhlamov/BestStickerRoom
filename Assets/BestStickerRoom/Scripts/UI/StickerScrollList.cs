using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using BestStickerRoom.Data;

namespace BestStickerRoom.UI
{
    public class StickerScrollList : MonoBehaviour
    {
        [SerializeField] private ScrollRect scrollRect;
        [SerializeField] private RectTransform contentContainer;
        [SerializeField] private StickerItemUI stickerItemPrefab;
        [SerializeField] private float itemSpacing = 10f;
        [SerializeField] private Vector2 itemSize = new Vector2(100f, 100f);

        private List<StickerItemUI> stickerItems = new List<StickerItemUI>();

        private void Awake()
        {
            if (scrollRect == null)
            {
                scrollRect = GetComponent<ScrollRect>();
            }

            if (contentContainer == null && scrollRect != null)
            {
                contentContainer = scrollRect.content;
            }

            if (contentContainer == null)
            {
                Debug.LogError("StickerScrollList: ContentContainer is not assigned and could not be found!");
            }

            if (scrollRect != null)
            {
                scrollRect.horizontal = true;
                scrollRect.vertical = false;
            }
        }

        private void SetStickers(StickerAsset[] assets)
        {
            ClearItems();

            if (assets == null || assets.Length == 0)
            {
                return;
            }
            
            foreach (var asset in assets)
            {
                if (!asset || !asset.Sprite) continue;

                CreateStickerItem(asset);
            }
        }

        public void SetStickerPack(StickerPack pack)
        {
            if (pack == null || pack.Stickers == null)
            {
                ClearItems();
                return;
            }

            var assets = new StickerAsset[pack.Stickers.Count];
            for (var i = 0; i < pack.Stickers.Count; i++)
            {
                assets[i] = pack.Stickers[i];
            }

            SetStickers(assets);
        }

        private StickerItemUI CreateStickerItem(StickerAsset asset)
        {
            if (stickerItemPrefab == null)
            {
                Debug.LogError("StickerScrollList: StickerItemPrefab is not assigned!");
                return null;
            }

            var stickerItem = Instantiate(stickerItemPrefab, contentContainer);
            stickerItem.Initialize(asset);
            stickerItem.gameObject.SetActive(true);
            return stickerItem;
        }

        private void ClearItems()
        {
            foreach (var item in stickerItems)
            {
                if (item != null)
                {
                    Destroy(item.gameObject);
                }
            }

            stickerItems.Clear();
        }
    }
}
