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
        [SerializeField] private GameObject stickerItemPrefab;
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

        public void SetStickers(StickerAsset[] assets)
        {
            ClearItems();

            if (assets == null || assets.Length == 0)
            {
                return;
            }

            var validCount = 0;
            for (var i = 0; i < assets.Length; i++)
            {
                if (assets[i] != null && assets[i].Sprite != null) validCount++;
            }

            var contentWidth = validCount * (itemSize.x + itemSpacing) - itemSpacing;
            contentContainer.sizeDelta = new Vector2(contentWidth, contentContainer.sizeDelta.y);

            var itemIndex = 0;
            for (var i = 0; i < assets.Length; i++)
            {
                if (assets[i] == null || assets[i].Sprite == null) continue;

                var item = CreateStickerItem(assets[i]);
                if (item != null)
                {
                    var rectTransform = item.GetComponent<RectTransform>();
                    if (rectTransform != null)
                    {
                        rectTransform.anchoredPosition = new Vector2(
                            itemIndex * (itemSize.x + itemSpacing) + itemSize.x * 0.5f,
                            0f
                        );
                        rectTransform.sizeDelta = itemSize;
                    }

                    stickerItems.Add(item);
                    itemIndex++;
                }
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

            var itemObject = Instantiate(stickerItemPrefab, contentContainer);
            var stickerItem = itemObject.GetComponent<StickerItemUI>();
            if (stickerItem == null)
            {
                stickerItem = itemObject.AddComponent<StickerItemUI>();
            }

            stickerItem.Initialize(asset);
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
