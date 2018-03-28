using HoloToolkit.Sharing.Tests;
using HoloToolkit.Unity;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameState : IState {

    private bool _finished = false;

    private TextToSpeech      _speecher;
    private SyncObjectSpawner _spawner;

    public override object[] GetParams() {
        return new object[] { };
    }

    public override bool IsFinished() {
        return _finished;
    }
}
