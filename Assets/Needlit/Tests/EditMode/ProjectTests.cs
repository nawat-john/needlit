using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace Needlit.Tests
{
    public sealed class ProjectTests
    {
        [Test]
        public void ProjectHasPlayableScenesAndUrp()
        {
            var scenes = EditorBuildSettings.scenes;
            Assert.AreEqual(3, scenes.Length);
            Assert.AreEqual("Assets/Needlit/Scenes/Bootstrap.unity", scenes[0].path);
            foreach (var scene in scenes)
                Assert.IsTrue(AssetDatabase.LoadAssetAtPath<SceneAsset>(scene.path), scene.path);
            Assert.IsNotNull(GraphicsSettings.defaultRenderPipeline);
        }
    }
}
