using System;

namespace SimpleMan.StateMachine
{
    public abstract class BlackboardBase<TKey> where TKey : Enum
    {
        public TKey nextStateKey;
    }
}

