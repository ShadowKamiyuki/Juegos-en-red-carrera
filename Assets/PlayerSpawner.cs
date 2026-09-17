//using UnityEngine;
//using Fusion;

//public class PlayerSpawner : SimulationBehaviour, IPlayerJoined
//{
//    public NetworkObject playerPrefab;
//    public Transform[] spawnPoints;

//    public void PlayerJoined(PlayerRef player)
//    {
//        if (player != Runner.LocalPlayer)
//            return;

//        int posicion = player.RawEncoded % spawnPoints.Length;

//        Runner.Spawn(
//            playerPrefab,
//            spawnPoints[posicion].position,
//            spawnPoints[posicion].rotation,
//            player
//        );
//        Debug.Log("Player Spawneado");
//    }
//}