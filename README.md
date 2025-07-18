# SimpleMan.StateMachine-BlackboardBased [Download](https://github.com/IgorChebotar/StateMachine-BlackboardBased/releases)
Blackboard-based state machine with async operation supporting

**Authors:** [Igor-Valerii Chebotar](https://www.linkedin.com/in/igor-chebotar/) 
<br>

# Requirements (all free)
* SimpleMan.Utilities
* Zenject
* UniTask

# Quick start
1. Create enum where key is state
```C# 
public enum EState
{
    MainMenu = 0,
    Gameplay,
    LoadingGameplay,
    ReturnToMenu,
    Paused,
    Result,
}
```

2. Create state classes
```C# 
public class ResultState : StateBase
{
    protected override UniTask StartAsync()
    {
        // this code will be executed on start of the state.
	// add async/await if you need async operation.
	// state can not be changed before async operation is done.

	//if you DON'T need to use async operation, just put 'return base.StartAsync()' 
        //at the end of the method.
    }

    protected override UniTask StopAsync()
    {
        
    }
}
```

3. When you added your state classes, you need to bind them with enum keys.
You can do it using attribute 'Bind'.
```C# 
public enum EState
{
    [Bind(typeof(MainMenuState))]
    MainMenu = 0,

    [Bind(typeof(GameplayState))]
    Gameplay,

    [Bind(typeof(LoadingGameplayState))]
    LoadingGameplay,

    [Bind(typeof(ReturnMainMenuState))]
    ReturnToMenu,

    [Bind(typeof(PausedState))]
    Paused,

    [Bind(typeof(ResultState))]
    Result,
}
```

4. Create Blackboard class. This class used by all states and contains 
common values. Base blackboard class already contains 'next state key' value. 
You can switch states using 'next state key' value. State machine process this operation
automatically. You can add your own values and use them inside states or by other classes.
```C# 
public class Blackboard : BlackboardBase<EState>
{
    public string gameplaySceneName = "Default";
}
```

5. Create wrapper class for base StateMachineFactory. Put here your enum and blackboard as generics.
```C# 
public class StateMachineFactory : StateMachineZenjectFactory<EState, Blackboard> { }
```

6. Bind trough Zenject this new wrapper class. And call method 'Create' on initialization of your application. Done!
