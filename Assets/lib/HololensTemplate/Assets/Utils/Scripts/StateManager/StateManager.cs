using System;
using System.Collections.Generic;

using HoloToolkit.Unity;

using UnityEngine;

namespace Assets.Scripts.StateManager {
    internal class StateManager : Singleton<StateManager> {
        public delegate void EventManager();

        public enum RunningState {
            Running,
            Stopped,
            Paused
        }

        [SerializeField]
        private readonly List<IState> _list = new List<IState>();

        private int _index;

        private RunningState      _running;
        public event EventManager OnStartWorking;
        public event EventManager OnElementChanging;
        public event EventManager OnFinishWorking;
        public event EventManager OnCancelWorking;
        public event EventManager OnPauseWorking;
        public event EventManager OnResumeWorking;

        public void Restart() {
            Stop();
            Launch(0);
        }

        public void Launch(int index) {
            if (_list.Count == 0) throw new StateManagerException("List array can't be empty");
            _index = index;
            _list[_index].OnStart();
            OnStartWorking?.Invoke();
            _running = RunningState.Running;
        }

        public void Pause() {
            if (_running == RunningState.Running) {
                _running = RunningState.Paused;
                OnPauseWorking?.Invoke();
            }
        }

        public void Resume() {
            if (_running == RunningState.Paused) {
                _running = RunningState.Running;
                OnResumeWorking?.Invoke();
            }
        }

        public void Stop() {
            if (_running == RunningState.Running) {
                _running = RunningState.Stopped;
                OnCancelWorking?.Invoke();
                _list[_index].OnCancel();
            }
        }

        public void Reset() {
            Stop();
            _index = 0;
        }

        public void Clear() {
            Stop();
            _list.Clear();
        }

        public void Add(IState state) {
            _list.Add(state);
        }

        public void Remove(IState state) {
            _list.Remove(state);
        }

        private void Update() {
            if (_running == RunningState.Running) {
                IState state = _list[_index];
                state.OnUpdate();
                if (state.IsFinished())
                    if (_index + 1 < _list.Count)
                        Next(state.GetParams());
                    else
                        OnFinishWorking?.Invoke();
            }
        }

        private void FixedUpdate() {
            if (_running == RunningState.Running)
                _list[_index].OnFixedUpdate();
        }

        private void Next(params object[] args) {
            _list[_index++].OnStop();
            OnElementChanging?.Invoke();
            _list[_index].OnStart(args);
            OnStartWorking?.Invoke();
        }

        public class StateManagerException : Exception {
            public StateManagerException(string message) : base(message) { }
        }
    }
}