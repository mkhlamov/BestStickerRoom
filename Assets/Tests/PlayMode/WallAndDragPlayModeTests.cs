using System.Collections;
using System.ComponentModel;
using System.Reflection;
using BestStickerRoom.Core;
using BestStickerRoom.Data;
using BestStickerRoom.Room;
using BestStickerRoom.UI;
using NUnit.Framework;
using UniRx;
using UnityEngine;
using UnityEngine.TestTools;
using Zenject;

namespace BestStickerRoom.Tests.PlayMode
{
    public class WallAndDragPlayModeTests : ZenjectIntegrationTestFixture
    {
        [UnityTest]
        public IEnumerator WallDetector_DetectWallFromWorldRay_ReturnsValidHit()
        {
            PreInstall();

            var cameraObject = new GameObject("RaycastCamera");
            var camera = cameraObject.AddComponent<Camera>();
            camera.orthographic = true;

            var inputObject = new GameObject("InputManager");
            var inputManager = inputObject.AddComponent<InputManager>();
            
            Container.BindInstance(camera).WithId("RaycastCamera");
            Container.BindInstance(inputManager);
            Container.BindInterfacesAndSelfTo<WallDetector>().AsSingle().NonLazy();

            PostInstall();

            var wall = new GameObject("Wall");
            wall.tag = "Room";
            wall.AddComponent<BoxCollider2D>();
            wall.transform.position = Vector3.zero;

            var detector = Container.Resolve<WallDetector>();

            var ray = new Ray(new Vector3(-1f, 0f, 0f), Vector3.right);
            var hit = detector.DetectWallFromWorldRay(ray);

            Assert.IsTrue(hit.IsValid);
            Assert.AreEqual(wall, hit.WallObject);

            yield return null;
        }

        [UnityTest]
        public IEnumerator StickerPlacer_CreatesSticker_OnDragAllowedAndDrop()
        {
            PreInstall();

            var cameraObject = new GameObject("RaycastCamera");
            var camera = cameraObject.AddComponent<Camera>();
            camera.orthographic = true;

            var gate = new TestDragDropGate();
            var dragDropHandlerObject = new GameObject("DragDropHandler");
            var dragDropHandler = dragDropHandlerObject.AddComponent<DragDropHandler>();

            var levelSettings = ScriptableObject.CreateInstance<LevelSettings>();
            var sprite = CreateSprite();
            var prefab = CreateStickerPrefab(sprite);

            SetPrivateField(levelSettings, "stickerPrefab", prefab);
            SetPrivateField(levelSettings, "stickerOffsetFromSurface", 0.01f);
            SetPrivateField(levelSettings, "stickerSize", new Vector2(1f, 1f));

            Container.BindInstance(levelSettings);
            Container.BindInstance(dragDropHandler);
            Container.Bind<IDragDropGate>().FromInstance(gate);
            Container.Bind<Camera>().WithId("RaycastCamera").FromInstance(camera);
            Container.Bind<StickerPlacer>().FromNewComponentOnNewGameObject().AsSingle().NonLazy();

            PostInstall();

            var placer = Container.Resolve<StickerPlacer>();

            var stickerAsset = ScriptableObject.CreateInstance<StickerAsset>();
            SetPrivateField(stickerAsset, "sprite", sprite);

            var dragData = new DragDropData
            {
                Data = new StickerData(stickerAsset),
                StartScreenPosition = new Vector2(100f, 100f),
                CurrentScreenPosition = new Vector2(100f, 100f)
            };

            GameObject placedSticker = null;
            placer.OnStickerPlaced += sticker => placedSticker = sticker;

            InvokePrivate(placer, "HandleDragStarted", dragData);
            gate.DragAllowed.Value = true;
            InvokePrivate(placer, "HandleDragUpdated", dragData);

            var wall = new GameObject("Wall");
            wall.tag = "Room";
            var wallHit = WallHitResult.Create(Vector3.zero, wall);

            InvokePrivate(placer, "HandleDragDropped", dragData, wallHit);

            Assert.IsNotNull(placedSticker);
            var renderer = placedSticker.GetComponentInChildren<SpriteRenderer>();
            Assert.IsNotNull(renderer);
            Assert.AreEqual(sprite, renderer.sprite);

            yield return null;
        }

        private static void InvokePrivate(object target, string methodName, params object[] args)
        {
            var method = target.GetType().GetMethod(methodName, BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.IsNotNull(method);
            method.Invoke(target, args);
        }

        private static void SetPrivateField<T>(object target, string fieldName, T value)
        {
            var field = target.GetType().GetField(fieldName, BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.IsNotNull(field);
            field.SetValue(target, value);
        }

        private static Sprite CreateSprite()
        {
            var texture = new Texture2D(2, 2);
            texture.SetPixels(new[] { Color.white, Color.white, Color.white, Color.white });
            texture.Apply();
            return Sprite.Create(texture, new Rect(0f, 0f, 2f, 2f), new Vector2(0.5f, 0.5f), 100f);
        }

        private static GameObject CreateStickerPrefab(Sprite sprite)
        {
            var prefab = new GameObject("StickerPrefab");
            var renderer = prefab.AddComponent<SpriteRenderer>();
            renderer.sprite = sprite;
            return prefab;
        }

        private sealed class TestDragDropGate : IDragDropGate
        {
            public ReactiveProperty<bool> DragAllowed { get; } = new(false);
        }
    }
}
