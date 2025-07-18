using SimpleMan.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Zenject;

namespace SimpleMan.StateMachine
{
    public class StateMachineZenjectFactory<TKey, TBlackboard>
        where TKey : Enum
        where TBlackboard : BlackboardBase<TKey>
    {
        [Inject] private readonly IInstantiator _container;
        [Inject] private readonly TBlackboard _blackboard;

        public virtual SimpleStateMachine<TKey, TBlackboard> Create(string name)
        {
            SimpleStateMachine<TKey, TBlackboard> stateMachine = _container.Instantiate<SimpleStateMachine<TKey, TBlackboard>>();

            Dictionary<TKey, StateBase<TBlackboard, TKey>> states = CreateStates(_blackboard, stateMachine);
            stateMachine.Init(name, _blackboard, states);

            return stateMachine;
        }

        private Dictionary<TKey, StateBase<TBlackboard, TKey>> CreateStates(TBlackboard blackboard, SimpleStateMachine<TKey, TBlackboard> stateMachine)
        {
            Dictionary<TKey, Type> bindedStates = GetBindedStates();
            Dictionary<TKey, StateBase<TBlackboard, TKey>> result = new Dictionary<TKey, StateBase<TBlackboard, TKey>>(bindedStates.Count);

            foreach (var pair in bindedStates)
            {
                StateBase<TBlackboard, TKey> state = CreateState<StateBase<TBlackboard, TKey>>(pair.Value, blackboard, stateMachine);
                result.Add(pair.Key, state);
            }

            return result;
        }

        public TState CreateState<TState>(Type type, TBlackboard blackboard, SimpleStateMachine<TKey, TBlackboard> stateMachine) where TState : StateBase<TBlackboard, TKey>
        {
            TState state = _container.Instantiate(type) as TState;
            state.Init(blackboard, stateMachine);

            return state;
        }

        private Dictionary<TKey, Type> GetBindedStates()
        {
            Array keys = Enum.GetValues(typeof(TKey));
            Dictionary<TKey, Type> result = new Dictionary<TKey, Type>(keys.Length);

            foreach (var key in keys)
            {
                if (TryGetBindedType(key, out Type bindedType))
                    result.Add((TKey)key, bindedType);
            }

            return result;
        }

        private bool TryGetBindedType(object key, out Type result)
        {
            result = null;
            MemberInfo memberInfo = typeof(TKey).GetMember(key.ToString()).FirstOrDefault();

            if (memberInfo.NotExist())
                return false;

            BindAttribute binderAttribute = (BindAttribute)memberInfo.GetCustomAttribute(typeof(BindAttribute), false);
            if (binderAttribute.NotExist())
                return false;

            result = binderAttribute.bindedType;
            return true;
        }
    }
}

