using System;
using Fusion;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelSelector : NetworkBehaviour
{
    [Header("Scene Indexes")]
    [SerializeField] private int level1Index = 1;
    [SerializeField] private int level2Index = 2;
    [SerializeField] private int level3Index = 3;

    public void SelectLevel1()
    {
        SelectLevel(level1Index);
    }

    public void SelectLevel2()
    {
        SelectLevel(level2Index);
    }

    public void SelectLevel3()
    {
        SelectLevel(level3Index);
    }

    private void SelectLevel(int sceneIndex)
    {
        if (!Runner.IsSceneAuthority)
        {
            Debug.LogWarning("Solo el Host puede seleccionar el nivel.");
            return;
        }

        SceneRef scene = SceneRef.FromIndex(sceneIndex);

        Debug.Log("Host seleccionó la escena: " + sceneIndex);

        Runner.LoadScene(scene, LoadSceneMode.Single);
    }
}