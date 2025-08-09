using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

partial class InputGatheringSystem : SystemBase
{
    private GameControls _controls;
    
    protected override void OnCreate()
    {
        _controls = new GameControls();
        _controls.Enable();
        EntityManager.CreateSingleton<InputDataComponent>();
    }

    protected override void OnUpdate()
    {
        RefRW<InputDataComponent> inputDataComponent = SystemAPI.GetSingletonRW<InputDataComponent>();
        float2 movementValue = _controls.Player.Move.ReadValue<Vector2>();
        //since movement value is used in fixed time systems, and input system currently updates in dynamic loop...
        //there will be desyncs, but it is acceptable for movement
        inputDataComponent.ValueRW.MovementData = movementValue;
    }

    protected override void OnDestroy()
    {
        _controls.Dispose();
    }
}
