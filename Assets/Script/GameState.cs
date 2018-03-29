using HoloToolkit.Sharing.Tests;
using HoloToolkit.Unity;
using System.Collections;
using System.Collections.Generic;

using HoloToolkit.Unity.InputModule;

using UnityEngine;
using UnityEngine.XR.WSA.Input;
using Assets.Scripts;

using HoloToolkit.Sharing;
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

        //_recognizer.Tapped += _recognizer_Tapped;

        StateRegistrer.Instance.OnTurnChange += Instance_OnTurnChange;

        if (StateRegistrer.Instance.hoster)
            StateRegistrer.Instance.game.playerTurn.Value = true;
    }

    private void Instance_OnTurnChange() {
        if (StateRegistrer.Instance.hoster == StateRegistrer.Instance.game.playerTurn.Value)
            return;

        // Win / lose checking
        bool gotOne = false;
        foreach (Transform childTransform in _spawner.SearchSyncObject(typeof(SyncGobeletsP1)).GameObject.transform) {
            if (childTransform.gameObject.activeSelf)
                gotOne = true;
        }

        if (!gotOne) {
            if (StateRegistrer.Instance.hoster)
                _speecher.StartSpeaking("You losed");
            else
                _speecher.StartSpeaking("You won");

            return;
        }

        gotOne = false;

        foreach (Transform childTransform in _spawner.SearchSyncObject(typeof(SyncGobeletsP2)).GameObject.transform)
        {
            if (childTransform.gameObject.activeSelf)
                gotOne = true;
        }

        if (!gotOne)
        {
            if (!StateRegistrer.Instance.hoster)
                _speecher.StartSpeaking("You losed");
            else
                _speecher.StartSpeaking("You won");

            return;
        }

        // Not finished so we continue

        _speecher.StartSpeaking("It's your turn");

        SyncBall ball = new SyncBall();
        Transform tr = Camera.main.transform;
        GameObject myTriangle = StateRegistrer.Instance.hoster ? _spawner.SearchSyncObject(typeof(SyncGobeletsP1)).GameObject : _spawner.SearchSyncObject(typeof(SyncGobeletsP2)).GameObject;
        Vector3 pos = new Vector3(myTriangle.transform.position.x, tr.position.y, myTriangle.transform.position.z);

        _spawner.SpawnSyncObject(ball, pos, Quaternion.identity);
    }

    //private void _recognizer_Tapped(TappedEventArgs obj) {
    //    GameObject go = GazeManager.Instance.HitObject;

    //    if (go == null)
    //        return;

    //    if (StateRegistrer.Instance.hoster == StateRegistrer.Instance.game.playerTurn.Value) return;

    //    SyncBall ball = (SyncBall)_spawner.SearchSyncObject(typeof(SyncBall));
    //    _spawner.DeleteSyncObject(ball);

    //    StateRegistrer.Instance.game.desactivedObjects.AddObject(new SyncObject(go.transform.parent.GetFullPath()));
    //    StateRegistrer.Instance.game.playerTurn.Value = !StateRegistrer.Instance.game.playerTurn.Value;
    //}

    public override void OnUpdate() {
        // Desactived object
        foreach (SyncObject so in StateRegistrer.Instance.game.desactivedObjects) {
            Debug.Log("Object path: " + so.FieldName);
            GameObject.Find(so.FieldName).SetActive(false);
        }

        Debug.Log("Hierarchy:");

        displayInfo(GameObject.Find("SpawnRoot"));
    }

    private void displayInfo(GameObject o) {
        bool test = false;
        foreach (Transform child in o.transform) {
            displayInfo(child.gameObject);
            test = true;
        }

        if (test) return;

        Debug.Log("Fullpath: " + o.transform.GetFullPath());
    }
}
