using Assets.Scripts.StateManager;

using HoloToolkit.Unity;

using UnityEngine;

public class StateRegistrer : Singleton<StateRegistrer> { // Un peu la main class :)
    public bool hoster;

    public SyncGame game;
    public bool lastTurn;

    public delegate void x();

    public event x OnTurnChange;

    private void Start() {
        lastTurn = false;

        StateManager.Instance.Add(new ConnectServerState());
        StateManager.Instance.Add(new ScanningState());
        StateManager.Instance.Add(new GameState());
        StateManager.Instance.Launch(0);
    }

    public void Update() {
        if (game == null)
            return;

        if (game.playerTurn.Value != lastTurn)
            OnTurnChange?.Invoke();

        lastTurn = game.playerTurn.Value;
    }
}