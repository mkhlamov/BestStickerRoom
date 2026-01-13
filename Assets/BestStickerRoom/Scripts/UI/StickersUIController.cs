using UnityEngine;
using BestStickerRoom.Data;
using Zenject;

namespace BestStickerRoom.UI
{
    public class StickersUIController : MonoBehaviour
    {
        [SerializeField] private StickerScrollList stickerScrollList;

        private LevelSettings levelSettings;

        private void Awake()
        {
            if (stickerScrollList == null)
            {
                stickerScrollList = GetComponent<StickerScrollList>();
            }

            if (stickerScrollList == null)
            {
                Debug.LogError("StickersUIController: StickerScrollList is not assigned and could not be found!");
            }
        }

        private void Start()
        {
            Initialize();
        }

        [Inject]
        private void Construct(LevelSettings settings)
        {
            levelSettings = settings;
        }

        private void Initialize()
        {
            if (levelSettings == null)
            {
                Debug.LogError("StickersUIController: LevelSettings is not assigned!");
                return;
            }

            if (stickerScrollList == null)
            {
                Debug.LogError("StickersUIController: StickerScrollList is not assigned!");
                return;
            }

            var stickerPack = levelSettings.StickerPack;
            if (stickerPack == null)
            {
                Debug.LogWarning("StickersUIController: StickerPack is null in LevelSettings!");
                return;
            }

            stickerScrollList.SetStickerPack(stickerPack);
        }
    }
}
