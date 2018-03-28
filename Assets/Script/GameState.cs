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

    /// <inheritdoc />
    public override void OnStart(params object[] args) {
        _speecher = args[0] as TextToSpeech;
        _spawner = args[1] as SyncObjectSpawner;
        
    }

    public override void OnUpdate() {
        if (StateRegistrer.Instance.hoster)
            StateRegistrer.Instance.game.posPlayer1 = Camera.main.transform.position;
        else {
            StateRegistrer.Instance.game.posPlayer2 = Camera.main.transform.position;
        }
    }
}
