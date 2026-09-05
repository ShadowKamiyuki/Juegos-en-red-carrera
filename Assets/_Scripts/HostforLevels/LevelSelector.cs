using Fusion;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSelector : NetworkBehaviour
{
    public void SelectLevel(int sceneIndex)
    {
        if (!Runner.IsSceneAuthority)
            return;

        SceneRef scene = SceneRef.FromIndex(sceneIndex);

        Runner.LoadScene(scene, LoadSceneMode.Single);
    }
}