using UnityEngine;
using UnityEngine.SceneManagement;

namespace Needlit
{
    public sealed class SceneInteraction : MonoBehaviour
    {
        public string scene;
        public GameMode mode = GameMode.Explore;

        public void Use(PlayerController player)
        {
            if (!string.IsNullOrEmpty(scene)) SceneManager.LoadScene(scene);
            else player.SetMode(mode);
        }
    }
}
