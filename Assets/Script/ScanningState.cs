using HoloToolkit.Sharing.Tests;
using HoloToolkit.Unity;
using HoloToolkit.Unity.SpatialMapping;
using UnityEngine;
using UnityEngine.XR.WSA.Input;

public class ScanningState : IState {
    private bool              _finished;
    private GestureRecognizer _recognizer;
    private TextToSpeech      _speecher;
    private SyncObjectSpawner _spawner;

    public override object[] GetParams() {
        return new object[] {_speecher, _spawner};
    }

    public override bool IsFinished() {
        return _finished;
    }

    public override void OnStart(params object[] args) {
        _speecher = GameObject.Find("TextToSpeechManager").GetComponent<TextToSpeech>();
        _spawner = GameObject.Find("SyncObjectSpawner").GetComponent<SyncObjectSpawner>();
        _recognizer = new GestureRecognizer();

        if (!StateRegistrer.Instance.hoster)
        {
            _speecher.StartSpeaking("Wait that your host configure the game");
            return;
        }

        _speecher.StartSpeaking("Scanning your environment, please wait !");

        _recognizer.Tapped += _recognizer_Tapped;

        _recognizer.StartCapturingGestures();

        

        SpatialMappingManager.Instance.StartObserver();
        SpatialUnderstanding.Instance.RequestBeginScanning();

        SpatialUnderstandingCustomMesh customMesh = SpatialUnderstanding.Instance.GetComponent<SpatialUnderstandingCustomMesh>();
        customMesh.DrawProcessedMesh = true;
    }

    public override void OnUpdate() {
        if (StateRegistrer.Instance.game == null) {
            StateRegistrer.Instance.game = _spawner.SearchSyncObject(typeof(SyncGame)) as SyncGame;
            _finished = StateRegistrer.Instance.game != null;
        }
    }

    private void _recognizer_Tapped(TappedEventArgs obj) {
        finish();
    }

    private void finish() {
        _recognizer.Tapped -= _recognizer_Tapped;
        _recognizer.StopCapturingGestures();
        _finished          =  true;
        SpatialMappingManager.Instance.StopObserver();
        SpatialUnderstanding.Instance.RequestFinishScan();

        SpatialUnderstandingCustomMesh customMesh = SpatialUnderstanding.Instance.GetComponent<SpatialUnderstandingCustomMesh>();
        customMesh.DrawProcessedMesh                    = false;
        SpatialMappingManager.Instance.DrawVisualMeshes = false;
    }
}