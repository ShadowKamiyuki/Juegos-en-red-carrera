using UnityEngine;
using Fusion;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    private NetworkRunner runner;

    private NetworkRunner GetRunner()
    {
        if (runner == null || !runner.IsRunning)
        {
            runner = FindFirstObjectByType<NetworkRunner>();
        }

        return runner;
    }

    public void CargarEscena1() => CargarEscena(1);
    public void CargarEscena2() => CargarEscena(2);
    public void CargarEscena3() => CargarEscena(3);

    private void CargarEscena(int sceneIndex)
    {
        NetworkRunner activeRunner = GetRunner();

        if (activeRunner == null)
        {
            Debug.LogError("No se encontró ningún NetworkRunner activo.");
            return;
        }

        if (!activeRunner.IsSceneAuthority)
        {
            Debug.LogWarning("Solo el host puede cargar el nivel.");
            return;
        }

        Debug.Log("Cargando nivel " + sceneIndex);
        activeRunner.LoadScene(SceneRef.FromIndex(sceneIndex), LoadSceneMode.Single);
    }
}