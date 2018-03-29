using System.Collections;
using System.Collections.Generic;

using HoloToolkit.Sharing.SyncModel;

using UnityEngine;

public class SyncObjectString : SyncObject {
    [SyncData] public SyncString fullPath;

    public SyncObjectString() { }

    public SyncObjectString(string path) {
        fullPath.Value = path;
    }
}
