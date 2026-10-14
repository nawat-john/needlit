using UnityEngine;
using UnityEngine.SceneManagement;

namespace Needlit
{
    public sealed class Bootstrap : MonoBehaviour
    {
        void Start()
        {
            // shortcut: settings and saves are deferred until M2, load the playable room now.
            SceneManager.LoadScene("Home");
        }
    }
}
