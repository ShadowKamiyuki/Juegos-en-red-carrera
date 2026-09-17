//using System;
//using Fusion;
//using UnityEngine;
//using UnityEngine.SceneManagement;

//public class LevelSelector : NetworkBehaviour
//{
//    [Header("Scene Indexes")]
//    [SerializeField] private int level1Index = 1;
//    [SerializeField] private int level2Index = 2;
//    [SerializeField] private int level3Index = 3;
//    private NetworkManager netManager;
//    public void SelectLevel1()
//    {
//        SelectLevel(level1Index);
//    }

//    public void SelectLevel2()
//    {
//        SelectLevel(level2Index);
//    }

//    public void SelectLevel3()
//    {
//        SelectLevel(level3Index);
//    }

//    private void SelectLevel(int sceneIndex)
//    {
//        netManager.LoadGameScene(sceneIndex);
//    }
//}