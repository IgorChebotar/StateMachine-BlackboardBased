using Cysharp.Threading.Tasks;
using System;

namespace SimpleMan.StateMachine
{
    public abstract class StateBase<TBlackboard, TKey> 
        where TKey : Enum
        where TBlackboard : BlackboardBase<TKey>
    {
        public bool IsStartProcessRunning { get; private set; }
        public bool IsStopProcessRunning { get; private set; }
        public bool IsStopped { get; private set; }
        public TBlackboard Blackboard { get; private set; }
        public SimpleStateMachine<TKey, TBlackboard> StateMachine { get; private set; }




        internal void Init(TBlackboard blackboard, SimpleStateMachine<TKey, TBlackboard> stateMachine)
        {
            Blackboard = blackboard;
            StateMachine = stateMachine;
        }



        internal async UniTask InteranalStart()
        {
            IsStopped = false;
            IsStartProcessRunning = true;

            await StartAsync();
            IsStartProcessRunning = false;
        }

        internal async UniTask InteranalStop()
        {
            IsStopProcessRunning = true;
            await StopAsync();

            IsStopProcessRunning = false;
            IsStopped = true;
        }

        internal void InternalTick()
        {
            Tick();
        }

        internal void InternalFixedTick()
        {
            FixedTick();
        }

        protected virtual UniTask StartAsync()
        {
            return default;
        }

        protected virtual UniTask StopAsync()
        {
            return default;
        }

        protected virtual void Tick()
        {

        }

        protected virtual void FixedTick()
        {

        }
    }
}

