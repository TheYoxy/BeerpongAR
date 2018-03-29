using HoloToolkit.Sharing.Spawning;
using HoloToolkit.Sharing.SyncModel;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;

using UnityEngine;
using UnityEngine.Networking;

[SyncDataClass]
public class SyncGame : SyncSpawnedObject {
    [SyncData] public SyncBool playerTurn; // FALSE -> 1 / TRUE -> 1
    [SyncData] public SyncListString desactivedObject;
}
