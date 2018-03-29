using HoloToolkit.Sharing.Tests;
using HoloToolkit.Unity;
using System.Collections;
using System.Collections.Generic;

using HoloToolkit.Unity.InputModule;

using UnityEngine;
using UnityEngine.XR.WSA.Input;
using Assets.Scripts;

using HoloToolkit.Sharing.SyncModel;

public class GameState : IState {

    private bool _finished = false;

    private TextToSpeech      _speecher;
    private SyncObjectSpawner _spawner;
    private GestureRecognizer _recognizer;

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
        _recognizer = args[2] as GestureRecognizer;

        _speecher.StartSpeaking("You are now in the game state");

        _recognizer.Tapped += _recognizer_Tapped;

        StateRegistrer.Instance.OnTurnChange += Instance_OnTurnChange;

        //TODO INSTANTIATE 3D TEXT WITH BILLBOARD AND IN UPDATE SET PLAYER 2 OR 1 POS + 0.1 in Y
    }

    private void Instance_OnTurnChange()
    {
        _speecher.StartSpeaking("The turn changed !");
    }

    private void _recognizer_Tapped(TappedEventArgs obj) {
        GameObject go = GazeManager.Instance.HitObject;

        if (go == null)
            return;

        _speecher.StartSpeaking("Name of the object " + go.name);

        //DisplayInformation(go);
        //_speecher.StartSpeaking("Display information");
    }

    //private void DisplayInformation(GameObject go) {
    //    foreach (Transform transform in go.transform) {
    //        DisplayInformation(transform.gameObject);
    //    }

    //    Logs.Instance.WriteLogLineWarning("Displaying info of " + go.name);

    //    foreach (Component cpn in go.GetComponents<Component>()) {
    //        Logs.Instance.WriteLogLineWarning("Compnent Name: " + cpn.name + " type: " + cpn.GetType());
    //    }
    //}

    public override void OnUpdate() {
        if (StateRegistrer.Instance.hoster)
            StateRegistrer.Instance.game.posPlayer1.Value = Camera.main.transform.position;
        else {
            StateRegistrer.Instance.game.posPlayer2.Value = Camera.main.transform.position;
        }

        // Desactived object
        foreach (string s in StateRegistrer.Instance.game.desactivedObject) {
            GameObject.Find(s).SetActive(false);
        }
    }
}
