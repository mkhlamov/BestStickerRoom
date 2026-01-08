using BestStickerRoom.Data;
using UnityEngine;
using Zenject;

namespace BestStickerRoom.Installers
{
    [CreateAssetMenu(fileName = "LevelInstaller", menuName = "BestStickerRoom/LevelInstaller")]
    public class LevelInstaller : ScriptableObjectInstaller<LevelInstaller>
    {
        [SerializeField]
        private LevelSettings levelSettings;
        public override void InstallBindings()
        {
            Container.BindInstance(levelSettings).AsSingle();
        }
    }
}