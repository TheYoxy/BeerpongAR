using System;

using HoloToolkit.Sharing;
using HoloToolkit.Sharing.Utilities;

using UnityEngine;

public class ConnectServerState : IState {
    private bool _finished;

    private GameObject ManualIpConfigUtility;

    public override object[] GetParams() {
        return null;
    }

    public override bool IsFinished() {
        return _finished;
    }

    public override void OnStart(params object[] args) {
        ManualIpConfigUtility = GameObject.Find("ManualIpConfigUtility");
        ManualIpConfigUtility.SetActive(true);
        SharingStage.Instance.SharingManagerConnected += ConnectionCreated;
    }

    private void ConnectionCreated(object sender, EventArgs e) {
        _finished = true;
        StateRegistrer.Instance.hoster = ManualIpConfigUtility.GetComponent<ManualIpConfiguration>().IsHoster;
        ManualIpConfigUtility.SetActive(false);
        SharingStage.Instance.SharingManagerConnected -= ConnectionCreated;
    }
}