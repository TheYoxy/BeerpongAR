using HoloToolkit.Sharing.Tests;
using System.Collections;

using HoloToolkit.Unity;

using UnityEngine;
using HoloToolkit.Sharing.SyncModel;

public class Inside : MonoBehaviour {
    private const string    TAGBALL = "Ball";
    private       Coroutine Routine;

    private IEnumerator Oui(GameObject gobelet) {
        yield return new WaitForSeconds(.5f);

        if (StateRegistrer.Instance.crt != null)
            StopCoroutine(StateRegistrer.Instance.crt);

        SyncObjectSpawner _spawner = GameObject.Find("SyncObjectSpawner").GetComponent<SyncObjectSpawner>();

        SyncBall ball = (SyncBall) _spawner.SearchSyncObject(typeof(SyncBall));
        _spawner.DeleteSyncObject(ball);

        StateRegistrer.Instance.game.desactivedObjects.AddObject(new SyncObjectString(gobelet.transform.GetFullPath()));
        StateRegistrer.Instance.game.playerTurn.Value = !StateRegistrer.Instance.game.playerTurn.Value;
    }

    private void Start() { }

    private void Update() { }

    private void OnTriggerEnter(Collider collider) {
        if (collider.tag == TAGBALL)
            Routine = StartCoroutine(Oui(transform.parent.gameObject));
    }

    private void OnTriggerStay(Collider collider) { }

    private void OnTriggerExit(Collider collider) {
        if (collider.tag == TAGBALL)
            StopCoroutine(Routine);
    }
}