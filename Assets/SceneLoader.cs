using UnityEngine;
using Fusion;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public NetworkRunner runner;

    public void CargarEscena1()
    {
        if (runner.IsSceneAuthority)
        {
            runner.LoadScene(SceneRef.FromIndex(1), LoadSceneMode.Single);
        }
        
    }

    public void CargarEscena2()
    {
        if (runner.IsSceneAuthority)
        {
            runner.LoadScene(SceneRef.FromIndex(2), LoadSceneMode.Single);
        }
    }

    public void CargarEscena3()
    {
        if (runner.IsSceneAuthority)
        {
            runner.LoadScene(SceneRef.FromIndex(3), LoadSceneMode.Single);
        }
    }
}