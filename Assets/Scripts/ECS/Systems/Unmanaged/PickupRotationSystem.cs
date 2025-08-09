using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;

partial struct PickupRotationSystem : ISystem
{
    EntityQuery _query;
    
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        _query = new EntityQueryBuilder(Allocator.Temp)
            .WithAll<SpinningRotationComponent>()
            .WithAllRW<LocalTransform>()
            .Build(ref state);
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        NativeArray<Entity> entities = _query.ToEntityArray(Allocator.Temp);

        foreach (Entity entity in entities)
        {
            RefRO<SpinningRotationComponent> rotationComponent = SystemAPI.GetComponentRO<SpinningRotationComponent>(entity);
            
            RefRW<LocalTransform> localTransform = SystemAPI.GetComponentRW<LocalTransform>(entity);
            
            float rotationSpeed = math.radians(rotationComponent.ValueRO.AngleValuePerSecond) * SystemAPI.Time.DeltaTime;
            localTransform.ValueRW.Rotation = math.normalize(math.mul(quaternion.RotateY(rotationSpeed), localTransform.ValueRO.Rotation));
        }
        
        entities.Dispose();
    }
}
