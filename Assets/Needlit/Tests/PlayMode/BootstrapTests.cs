using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.LowLevel;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;

namespace Needlit.Tests
{
    public sealed class BootstrapTests
    {
        InputSettings.BackgroundBehavior previousBackgroundBehavior;
        InputSettings.EditorInputBehaviorInPlayMode previousEditorInputBehavior;

        [SetUp]
        public void RouteHeadlessInputToGame()
        {
            previousBackgroundBehavior = InputSystem.settings.backgroundBehavior;
            previousEditorInputBehavior = InputSystem.settings.editorInputBehaviorInPlayMode;
            InputSystem.settings.backgroundBehavior = InputSettings.BackgroundBehavior.IgnoreFocus;
            InputSystem.settings.editorInputBehaviorInPlayMode = InputSettings.EditorInputBehaviorInPlayMode.AllDeviceInputAlwaysGoesToGameView;
        }

        [TearDown]
        public void RestoreInputRouting()
        {
            InputSystem.settings.backgroundBehavior = previousBackgroundBehavior;
            InputSystem.settings.editorInputBehaviorInPlayMode = previousEditorInputBehavior;
        }

        [UnityTest]
        public IEnumerator BootstrapOpensHomeAndPortalOpensLab()
        {
            SceneManager.LoadScene("Bootstrap");
            yield return null;
            yield return null;
            Assert.AreEqual("Home", SceneManager.GetActiveScene().name);
            var player = Object.FindFirstObjectByType<PlayerController>();
            Assert.IsNotNull(player);
            var portal = Object.FindFirstObjectByType<SceneInteraction>();
            Assert.IsNotNull(portal);
            portal.Use(player);
            yield return null;
            Assert.AreEqual("GrindingLab", SceneManager.GetActiveScene().name);
            player = Object.FindFirstObjectByType<PlayerController>();
            var bench = GameObject.Find("Grinding Bench").GetComponent<SceneInteraction>();
            bench.Use(player);
            Assert.AreEqual(GameMode.Grinding, player.Mode);
        }

        [UnityTest]
        public IEnumerator ExploreMovesAndOtherModesStopMovement()
        {
            SceneManager.LoadScene("Home");
            yield return null;
            var player = Object.FindFirstObjectByType<PlayerController>();
            player.SendMessage("OnApplicationFocus", true);
            var keyboard = InputSystem.AddDevice<Keyboard>();
            try
            {
                float start = player.transform.position.z;
                InputSystem.QueueStateEvent(keyboard, new KeyboardState(Key.W));
                InputSystem.Update();
                Assert.IsTrue(keyboard.wKey.isPressed);
                yield return new WaitForSeconds(.12f);
                Assert.Greater(player.transform.position.z, start + .05f);

                player.SetMode(GameMode.Grinding);
                start = player.transform.position.z;
                yield return new WaitForSeconds(.12f);
                Assert.AreEqual(start, player.transform.position.z, .001f);

                player.SetMode(GameMode.Paused);
                yield return new WaitForSeconds(.12f);
                Assert.AreEqual(start, player.transform.position.z, .001f);
            }
            finally
            {
                InputSystem.RemoveDevice(keyboard);
            }
        }

        [UnityTest]
        public IEnumerator ReturningToExploreDiscardsFirstMouseDelta()
        {
            SceneManager.LoadScene("Home");
            yield return null;
            var player = Object.FindFirstObjectByType<PlayerController>();
            player.SendMessage("OnApplicationFocus", true);
            var mouse = InputSystem.AddDevice<Mouse>();
            try
            {
                player.SetMode(GameMode.Grinding);
                player.SetMode(GameMode.Explore);
                float yaw = player.transform.eulerAngles.y;
                InputSystem.QueueDeltaStateEvent(mouse.delta, new Vector2(100f, 0f));
                InputSystem.Update();
                player.SendMessage("Update");
                Assert.AreEqual(yaw, player.transform.eulerAngles.y, .001f);
                InputSystem.QueueDeltaStateEvent(mouse.delta, new Vector2(100f, 0f));
                InputSystem.Update();
                player.SendMessage("Update");
                Assert.AreNotEqual(yaw, player.transform.eulerAngles.y);
            }
            finally
            {
                InputSystem.RemoveDevice(mouse);
            }
        }
    }
}
