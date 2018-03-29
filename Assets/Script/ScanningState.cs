using System.Collections.Generic;

using cakeslice;

using HoloToolkit.Sharing;
using HoloToolkit.Sharing.SyncModel;
using HoloToolkit.Sharing.Tests;
using HoloToolkit.Unity;
using HoloToolkit.Unity.InputModule;
using HoloToolkit.Unity.SpatialMapping;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.XR.WSA.Input;

public class ScanningState : IState {
    private bool              _finished;
    private GestureRecognizer _recognizer;
    private TextToSpeech      _speecher;
    private SyncObjectSpawner _spawner;

    private List<GameObject> _tables;

    public override object[] GetParams() {
        return new object[] {_speecher, _spawner, _recognizer};
    }

    public override bool IsFinished() {
        return _finished;
    }

    public override void OnStart(params object[] args) {
        _speecher = GameObject.Find("TextToSpeechManager").GetComponent<TextToSpeech>();
        _spawner = GameObject.Find("SyncObjectSpawner").GetComponent<SyncObjectSpawner>();
        _recognizer = new GestureRecognizer();

        if (!StateRegistrer.Instance.hoster) {
            _speecher.StartSpeaking("Wait that your host configure the game");
            return;
        }

        _speecher.StartSpeaking("Scanning your environment, please wait");

        _recognizer.Tapped += _recognizer_Tapped;

        _recognizer.StartCapturingGestures();

        SpatialMappingManager.Instance.StartObserver();
    }

    public override void OnUpdate() { // If not the hoster we wait for the game to be created
        if (StateRegistrer.Instance.game == null && !StateRegistrer.Instance.hoster) {
            StateRegistrer.Instance.game = _spawner.SearchSyncObject(typeof(SyncGame)) as SyncGame;
            _finished = StateRegistrer.Instance.game != null;
        }
    }

    private void _recognizer_Tapped(TappedEventArgs obj) {
        if (_tables != null) {
            GameObject go = GazeManager.Instance.HitObject;

            if (!_tables.Contains(go))
                return;

            if (SharingStage.Instance.CurrentRoom.GetUserCount() != 2)
            {
                _speecher.StartSpeaking("Incorrect number of player this game require two players exactly !");
                return;
            }

            _speecher.StartSpeaking("Creating game");

            // Destroying outline
            foreach (GameObject got in _tables) {
                GameObject.Destroy(got.GetComponent<Outline>());
            }

            SyncGame game = new SyncGame();
            game.playerTurn.Value = false;
            _spawner.SpawnSyncObject(game, Vector3.zero, Quaternion.identity);
            StateRegistrer.Instance.game = game;

            SurfacePlane sp = go.GetComponent<SurfacePlane>();
            OrientedBoundingBox obb = sp.Plane.Bounds;

            // Which axes is better ?
            Vector3 dir;

            if (obb.Extents.x >= obb.Extents.z)
                dir = new Vector3(obb.Extents.x, 0, 0);
            else
                dir = new Vector3(0, 0, obb.Extents.z);

            dir = go.transform.TransformDirection(dir) / 1.5f; // Local to Global and resize a little

            SyncGobeletsP1 gobelets = new SyncGobeletsP1();
            _spawner.SpawnSyncObject(gobelets, obb.Center + dir, Quaternion.LookRotation(-dir));
            SyncGobeletsP2 gobelets2 = new SyncGobeletsP2();
            _spawner.SpawnSyncObject(gobelets2, obb.Center - dir, Quaternion.LookRotation(dir));

            finish();
        }
        else {
            SpatialMappingManager.Instance.StopObserver();

            // Now search if not found begin again
            SurfaceMeshesToPlanes.Instance.MakePlanesComplete += Instance_MakePlanesComplete;

            SurfaceMeshesToPlanes.Instance.MakePlanes();

            _speecher.StartSpeaking("Making planes, please wait");
        }
    }

    private void Instance_MakePlanesComplete(object source, System.EventArgs args)
    {
        SurfaceMeshesToPlanes.Instance.MakePlanesComplete -= Instance_MakePlanesComplete;

        _tables = SurfaceMeshesToPlanes.Instance.GetActivePlanes(PlaneTypes.Table);

        if (_tables.Count == 0) {
            _tables = null;
            _speecher.StartSpeaking("No table found, scanning the room again");
            SpatialMappingManager.Instance.StartObserver();
            return;
        }

        _speecher.StartSpeaking("Table found, select one");

        // Putting outline
        foreach (GameObject go in _tables) {
            Outline outl = go.AddComponent<Outline>();
            outl.color = 0;
        }
    }

    private void finish() {
        _recognizer.Tapped -= _recognizer_Tapped;
        _finished          =  true;

        SpatialMappingManager.Instance.DrawVisualMeshes = false;
    }
}