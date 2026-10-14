using System;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Needlit.Editor
{
    public static class ProjectSetup
    {
        const string Scenes = "Assets/Needlit/Scenes/";

        [MenuItem("NEEDLIT/Create M0 Scenes")]
        public static void CreateScenes()
        {
            if (!Application.isBatchMode && !EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                return;
            if (!AssetDatabase.IsValidFolder(Scenes.TrimEnd('/')))
                AssetDatabase.CreateFolder("Assets/Needlit", "Scenes");
            CreateBootstrap();
            CreateRoom("Home", false);
            CreateRoom("GrindingLab", true);
            AssetDatabase.Refresh();
            EditorBuildSettings.scenes = new[]
            {
                new EditorBuildSettingsScene(Scenes + "Bootstrap.unity", true),
                new EditorBuildSettingsScene(Scenes + "Home.unity", true),
                new EditorBuildSettingsScene(Scenes + "GrindingLab.unity", true)
            };
            AssetDatabase.SaveAssets();
            Debug.Log("NEEDLIT M0 scenes created.");
        }

        static void CreateBootstrap()
        {
            if (File.Exists(Scenes + "Bootstrap.unity")) return;
            EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            new GameObject("Bootstrap").AddComponent<Bootstrap>();
            EditorSceneManager.SaveScene(SceneManager.GetActiveScene(), Scenes + "Bootstrap.unity");
        }

        static void CreateRoom(string name, bool lab)
        {
            if (File.Exists(Scenes + name + ".unity")) return;
            EditorSceneManager.NewScene(NewSceneSetup.EmptyScene, NewSceneMode.Single);
            RenderSettings.ambientLight = new Color(.55f, .58f, .65f);
            var sun = new GameObject("Sun").AddComponent<Light>();
            sun.type = LightType.Directional;
            sun.intensity = 1.4f;
            sun.transform.rotation = Quaternion.Euler(50f, -30f, 0f);

            Block("Floor", new Vector3(0f, -.25f, 0f), new Vector3(14f, .5f, 14f), new Color(.27f, .28f, .26f));
            Block("Back Wall", new Vector3(0f, 2f, 7f), new Vector3(14f, 4f, .3f), new Color(.36f, .35f, .31f));
            Block("Front Wall", new Vector3(0f, 2f, -7f), new Vector3(14f, 4f, .3f), new Color(.36f, .35f, .31f));
            Block("Left Wall", new Vector3(-7f, 2f, 0f), new Vector3(.3f, 4f, 14f), new Color(.36f, .35f, .31f));
            Block("Right Wall", new Vector3(7f, 2f, 0f), new Vector3(.3f, 4f, 14f), new Color(.36f, .35f, .31f));

            var player = new GameObject("Player");
            player.transform.position = new Vector3(0f, 1f, -3f);
            var body = player.AddComponent<CharacterController>();
            body.height = 2f;
            body.radius = .35f;
            var camera = new GameObject("View").AddComponent<Camera>();
            camera.tag = "MainCamera";
            camera.transform.SetParent(player.transform, false);
            camera.transform.localPosition = new Vector3(0f, .65f, 0f);
            camera.gameObject.AddComponent<AudioListener>();
            player.AddComponent<PlayerController>();

            if (lab)
            {
                Block("Grinding Bench", new Vector3(0f, .7f, 3f), new Vector3(2f, 1.4f, 1.2f), new Color(.24f, .3f, .34f))
                    .AddComponent<SceneInteraction>().mode = GameMode.Grinding;
                var stock = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
                stock.name = "Iron Stock 300 mm diameter x 1000 mm (M1 placeholder)";
                stock.transform.position = new Vector3(0f, 1.55f, 3f);
                stock.transform.rotation = Quaternion.Euler(90f, 0f, 0f);
                stock.transform.localScale = new Vector3(.3f, .5f, .3f);
                UnityEngine.Object.DestroyImmediate(stock.GetComponent<Collider>());
                Paint(stock, new Color(.48f, .52f, .55f));
                Portal("Home", new Vector3(-2.5f, 1f, 4.5f), "Home", new Color(.2f, .48f, .38f));
            }
            else
            {
                Block("Home Table", new Vector3(-2.5f, .6f, 3f), new Vector3(2f, 1.2f, 1f), new Color(.44f, .27f, .16f));
                Portal("GrindingLab", new Vector3(0f, 1f, 4.5f), "GrindingLab", new Color(.3f, .55f, .75f));
            }
            EditorSceneManager.SaveScene(SceneManager.GetActiveScene(), Scenes + name + ".unity");
        }

        static GameObject Block(string name, Vector3 position, Vector3 scale, Color color)
        {
            var block = GameObject.CreatePrimitive(PrimitiveType.Cube);
            block.name = name;
            block.transform.position = position;
            block.transform.localScale = scale;
            Paint(block, color);
            return block;
        }

        static void Portal(string name, Vector3 position, string destination, Color color)
        {
            var portal = Block("E: " + name, position, new Vector3(1.4f, 2f, .3f), color);
            portal.AddComponent<SceneInteraction>().scene = destination;
        }

        static void Paint(GameObject target, Color color)
        {
            var shader = Shader.Find("Universal Render Pipeline/Lit");
            if (shader == null) throw new InvalidOperationException("URP Lit shader is unavailable.");
            if (!AssetDatabase.IsValidFolder("Assets/Needlit/Materials"))
                AssetDatabase.CreateFolder("Assets/Needlit", "Materials");
            string path = "Assets/Needlit/Materials/" + ColorUtility.ToHtmlStringRGB(color) + ".mat";
            var material = AssetDatabase.LoadAssetAtPath<Material>(path);
            if (material == null)
            {
                material = new Material(shader) { color = color };
                AssetDatabase.CreateAsset(material, path);
            }
            target.GetComponent<Renderer>().sharedMaterial = material;
        }

        [MenuItem("NEEDLIT/Build Windows")]
        public static void BuildWindows()
        {
            var scenes = EditorBuildSettings.scenes.Where(s => s.enabled).Select(s => s.path).ToArray();
            if (scenes.Length < 3) throw new InvalidOperationException("Run NEEDLIT/Create M0 Scenes first.");
            Directory.CreateDirectory("Builds/Windows");
            var report = BuildPipeline.BuildPlayer(new BuildPlayerOptions
            {
                scenes = scenes,
                locationPathName = "Builds/Windows/NEEDLIT.exe",
                target = BuildTarget.StandaloneWindows64,
                options = BuildOptions.None
            });
            if (report.summary.result != UnityEditor.Build.Reporting.BuildResult.Succeeded)
                throw new Exception("Windows build failed: " + report.summary.result);
            Debug.Log("NEEDLIT Windows build succeeded: " + report.summary.outputPath);
        }
    }
}
