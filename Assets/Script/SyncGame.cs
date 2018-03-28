using HoloToolkit.Sharing.Spawning;
using HoloToolkit.Sharing.SyncModel;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[SyncDataClass]
public class SyncGame : SyncSpawnedObject {
    [SyncData] public int scorePlayer1;
    [SyncData] public int scorePlayer2;
    [SyncData] public bool playerTurn; // FALSE -> 1 / TRUE -> 1
}
