using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;

[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
[UpdateAfter(typeof(ObjectPickupSystem))]
public partial struct PickupActivationSystem : ISystem
{
    private EntityQuery _disabledEntitiesQuery;
    private EntityQuery _notificationComponentRemovalQuery;
    private Random _random;
    
    private bool _initialized;
    
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        _random = new Random(57810658);
        
        _disabledEntitiesQuery = new EntityQueryBuilder(Allocator.Temp)
            .WithAll<PickupObjectComponent>()
            .WithOptions(EntityQueryOptions.IncludeDisabledEntities)
            .Build(ref state);
        
        _notificationComponentRemovalQuery = new EntityQueryBuilder(Allocator.Temp)
            .WithAll<PickupActivationRequestComponent>()
            .Build(ref state);
        
        Entity entity = state.EntityManager.CreateEntity();
        state.EntityManager.AddComponent<PickupActivationRequestComponent>(entity);
        state.EntityManager.CreateSingleton<ScorePointsComponent>();
        
        state.RequireForUpdate<PickupActivationRequestComponent>();
        state.RequireForUpdate<ScorePointsComponent>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        NativeArray<Entity> entities = _disabledEntitiesQuery.ToEntityArray(Allocator.Temp);
        int randomIndex = _random.NextInt(0, entities.Length);
        state.EntityManager.SetEnabled(entities[randomIndex], true);
        
        if(_initialized)
            SystemAPI.GetSingletonRW<ScorePointsComponent>().ValueRW.Value++;
        
        state.EntityManager.DestroyEntity(_notificationComponentRemovalQuery);
        _initialized = true;
        
        entities.Dispose();
    }
}