using ECS.Components;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
[UpdateAfter(typeof(ObjectPickupSystem))]
public partial struct PickupActivationSystem : ISystem
{
    public EntityQuery _enabledEntitiesQuery;
    public EntityQuery _disabledEntitiesQuery;
    public EntityQuery _notificationComponentRemovalQuery;
    private Random _random;
    
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        Entity entity = state.EntityManager.CreateEntity();
        state.EntityManager.AddComponent<PickupPerformedNotificationComponent>(entity);
        
        _random = new Random(57810658);
        
        _enabledEntitiesQuery = new EntityQueryBuilder(Allocator.Temp)
            .WithAll<PickupObjectComponent>()
            .Build(ref state);
        
        _disabledEntitiesQuery = new EntityQueryBuilder(Allocator.Temp)
            .WithAll<PickupObjectComponent>()
            .WithOptions(EntityQueryOptions.IncludeDisabledEntities)
            .Build(ref state);
        
        _notificationComponentRemovalQuery = new EntityQueryBuilder(Allocator.Temp)
            .WithAll<PickupPerformedNotificationComponent>()
            .Build(ref state);
        
        state.RequireForUpdate<PickupPerformedNotificationComponent>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        if (_enabledEntitiesQuery.IsEmpty)
        {
            NativeArray<Entity> entities = _disabledEntitiesQuery.ToEntityArray(Allocator.Temp);
            int randomIndex = _random.NextInt(0, entities.Length);
            state.EntityManager.SetEnabled(entities[randomIndex], true);

            entities.Dispose();
        }
        
        state.EntityManager.DestroyEntity(_notificationComponentRemovalQuery);
    }

    [BurstCompile]
    public void OnDestroy(ref SystemState state)
    {

    }
}