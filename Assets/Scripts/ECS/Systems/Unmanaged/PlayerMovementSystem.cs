using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Physics;
using Unity.Physics.Systems;
using Unity.Transforms;

[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
[UpdateBefore(typeof(PhysicsSystemGroup))]
partial struct PlayerMovementSystem : ISystem
{
    private EntityQuery _query;
    
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        _query = new EntityQueryBuilder(Allocator.Temp)
            .WithAll<PlayerComponent>()
            .WithAllRW<PhysicsVelocity>()
            .Build(ref state);
        
        state.RequireForUpdate<InputDataComponent>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        InputDataComponent inputDataComponent = SystemAPI.GetSingleton<InputDataComponent>();

        if (inputDataComponent.MovementData.x != 0f || inputDataComponent.MovementData.y != 0f)
        {
            float3 movementVector = math.normalizesafe(new float3(inputDataComponent.MovementData.x, 0f, inputDataComponent.MovementData.y));
            NativeArray<Entity> entities = _query.ToEntityArray(Allocator.Temp);

            foreach (var entity in entities)
            {
                RefRW<PhysicsVelocity> physicsVelocity = SystemAPI.GetComponentRW<PhysicsVelocity>(entity);
                RefRO<PlayerComponent> playerData = SystemAPI.GetComponentRO<PlayerComponent>(entity);

                physicsVelocity.ValueRW.Linear += movementVector * playerData.ValueRO.PlayerSpeed * SystemAPI.Time.DeltaTime;
            }

            entities.Dispose();
        }
    }
}
