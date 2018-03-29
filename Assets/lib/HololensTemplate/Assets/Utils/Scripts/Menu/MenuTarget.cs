using HoloToolkit.Unity.InputModule;

using UnityEngine;

public class MenuTarget : MonoBehaviour, IInputHandler {
    public MenuSpawner ms { get; set; }

    public void OnInputDown(InputEventData eventData) {
        ms.SendMessage("OnTargetClicked");
    }

    public void OnInputUp(InputEventData eventData) { }
}