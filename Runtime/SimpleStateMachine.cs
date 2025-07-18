using Cysharp.Threading.Tasks;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace SimpleMan.StateMachine
{
    public class SimpleStateMachine<TKey, TBlackboard> 
        where TKey : Enum
        where TBlackboard : BlackboardBase<TKey>
    {
        private Dictionary<TKey, StateBase<TBlackboard, TKey>> _states;

        public string Name { get; set; } = "Untitled state machine";
        public bool IsInitialized { get; private set; }
        public TBlackboard Blackboard { get; private set; }
        public IReadOnlyDictionary<TKey, StateBase<TBlackboard, TKey>> States { get; private set; }
        public TKey CurrentStateKey { get; private set; }


        internal void Init(string name, TBlackboard blackboard, Dictionary<TKey, StateBase<TBlackboard, TKey>> states)
        {
            if (IsInitialized)
            {
                return;
            }

            Name = name;
            IsInitialized = true;

            Blackboard = blackboard;
            _states = states;

            SwitchToFirstState();
            StartTicking();

            StartTickingFixed();
        }

        internal void Tick()
        {
            _states[CurrentStateKey].InternalTick();

            if (CurrentStateKey.Equals(Blackboard.nextStateKey))
            {
                return;
            }

            Debug.Log($"State machine '{Name}': Switching state process started. {CurrentStateKey} -> {Blackboard.nextStateKey}");

            if (_states[CurrentStateKey].IsStartProcessRunning ||
                _states[CurrentStateKey].IsStopProcessRunning)
            {
                return;
            }

            _states[CurrentStateKey].InteranalStop();
            if (!_states[CurrentStateKey].IsStopped)
            {
                return;
            }

            Debug.Log($"State machine '{Name}': State switched. {CurrentStateKey} -> {Blackboard.nextStateKey}");

            CurrentStateKey = Blackboard.nextStateKey;
            _states[CurrentStateKey].InteranalStart();
        }

        internal void FixedTick()
        {
            _states[CurrentStateKey].InternalFixedTick();
        }

        private async void StartTicking()
        {
            while (Application.isPlaying)
            {
                await UniTask.Yield(PlayerLoopTiming.Update);
                Tick();
            }
        }

        private async void StartTickingFixed()
        {
            while (Application.isPlaying)
            {
                await UniTask.WaitForFixedUpdate();
                FixedTick();
            }
        }

        private void SwitchToFirstState()
        {
            Debug.Log($"State machine '{Name}': State switched. None -> {Blackboard.nextStateKey}");
            _states[CurrentStateKey].InteranalStart();
        }
    }
}

