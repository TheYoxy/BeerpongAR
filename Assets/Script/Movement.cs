using HoloToolkit.Unity.InputModule;

using UnityEngine;

namespace Assets.Scripts {
    public abstract class Movement : MonoBehaviour, INavigationHandler, ISourceStateHandler {
        [Tooltip("Transform of the mouvement")]
        public Transform HostTransform;

        [Tooltip("Sensitivity of the mouvement")]
        public float Sensitivity = 1.0f;

        protected Vector3 factor = Vector3.zero;

        protected Vector3 navigationDelta = Vector3.zero;
        
        [SerializeField]
        private   bool    _enabled = true;

        public bool Enabled {
            get { return _enabled; }
            set {
                _enabled = value;
                if (!value)
                    Cancel();
            }
        }

        public void OnNavigationCanceled(NavigationEventData eventData) {
            if (Enabled) Cancel();
        }

        public void OnNavigationCompleted(NavigationEventData eventData) {
            if (Enabled) Cancel();
        }

        public void OnNavigationStarted(NavigationEventData eventData) {
            if (Enabled) {
                InputManager.Instance.OverrideFocusedObject = gameObject;
                navigationDelta                             = eventData.NormalizedOffset;
            }
        }

        public void OnNavigationUpdated(NavigationEventData eventData) {
            if (Enabled)
                navigationDelta = eventData.NormalizedOffset;
        }

        public void OnSourceDetected(SourceStateEventData eventData) { //Nothing to do
        }

        public void OnSourceLost(SourceStateEventData eventData) {
            if (Enabled)
                Cancel();
        }

        protected abstract void Perform();

        private void Cancel() {
            navigationDelta                             = Vector3.zero;
            InputManager.Instance.OverrideFocusedObject = null;
        }

        private void Update() {
            if (navigationDelta != Vector3.zero)
                Perform();
        }

        private void Awake() {
            if (HostTransform == null)
                HostTransform = transform;
        }
    }
}