using HoloToolkit.Sharing.Spawning;
using HoloToolkit.Sharing.SyncModel;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;

using UnityEngine;

[SyncDataClass]
public class SyncGame : SyncSpawnedObject {
    [SyncData] public bool playerTurn; // FALSE -> 1 / TRUE -> 1
    [SyncData] public Vector3 posPlayer1;
    [SyncData] public Vector3 posPlayer2;
}
