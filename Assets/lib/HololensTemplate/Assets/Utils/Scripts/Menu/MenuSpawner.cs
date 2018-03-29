using HoloToolkit.Unity;
using UnityEngine;

public class MenuSpawner : MonoBehaviour {
    [Tooltip("The angle where the UI will pop")]
    public int angle = 90;

    [Tooltip("Menu canvas to spawn once the player is in range")]
    public GameObject menu;

    [Tooltip("Offset between Menu canvas and target")]
    public float offset = 0.2f;

    [Tooltip("Pop menu on click and not when in range")]
    public bool popMenuClick = false;

    [Tooltip("Meter range that we need to be in to display the menu")]
    public int range = 10;

    [Tooltip("On which GameObject the menu should spawn")]
    public GameObject target;

    private bool menuVisible;

    public void OnTargetClicked() {
        menuVisible = !menuVisible;
    }

    private void Awake() {
        if (menu == null || target == null)
            return;

        if (popMenuClick) {
            MenuTarget mt = target.AddComponent<MenuTarget>();
            mt.ms = this;
        }
    }

    private void Update() {
        if (menu == null || target == null) {
            if (menu != null)
                menu.SetActive(false);
            return;
        }

        Vector3 sub = target.transform.position - Camera.main.transform.position;

        float distance = sub.magnitude;

        if (!popMenuClick) menuVisible = distance <= range;

        if (menuVisible) {
            Quaternion quat = Quaternion.Euler(new Vector3(0, angle, 0));
            sub.Normalize();

            Vector3 menupos = target.transform.position + quat * sub * offset;

            menu.transform.position = menupos;

            if (menu.GetComponent<Billboard>() == null)
                menu.AddComponent<Billboard>();

            menu.SetActive(true);
        } else { menu.SetActive(false); }
    }
}