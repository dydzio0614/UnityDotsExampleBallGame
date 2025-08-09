using ECS.Components;
using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Physics;
using Unity.Physics.Systems;

[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
[UpdateAfter(typeof(PhysicsSystemGroup))]
partial struct ObjectPickupSystem : ISystem
{
    private ComponentLookup<PickupObjectComponent> _pickupObjectLookup;
    
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        _pickupObjectLookup = SystemAPI.GetComponentLookup<PickupObjectComponent>(true);
        
        state.RequireForUpdate<SimulationSingleton>();
        state.RequireForUpdate<EndFixedStepSimulationEntityCommandBufferSystem.Singleton>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        _pickupObjectLookup.Update(ref state);
        SimulationSingleton simulation = SystemAPI.GetSingleton<SimulationSingleton>();
        EntityCommandBuffer commandBufferSystem = SystemAPI.GetSingleton<EndFixedStepSimulationEntityCommandBufferSystem.Singleton>()
            .CreateCommandBuffer(state.WorldUnmanaged);
        
        ObjectPickupTriggerEventsJob job = new ObjectPickupTriggerEventsJob()
        {
            PickupObjectsLookup = _pickupObjectLookup,
            Ecb = commandBufferSystem
        };

        state.Dependency = job.Schedule(simulation, state.Dependency);
    }

    [BurstCompile]
    private struct ObjectPickupTriggerEventsJob : ITriggerEventsJob
    {
        [ReadOnly]
        public ComponentLookup<PickupObjectComponent> PickupObjectsLookup;
        public EntityCommandBuffer Ecb;
        
        public void Execute(TriggerEvent triggerEvent)
        {
            Entity objectToPickup = PickupObjectsLookup.HasComponent(triggerEvent.EntityA) ? triggerEvent.EntityA 
                : PickupObjectsLookup.HasComponent(triggerEvent.EntityB) ? triggerEvent.EntityB 
                : Entity.Null;

            if (objectToPickup != Entity.Null)
            {
                Ecb.SetEnabled(objectToPickup, false);
                Entity entity = Ecb.CreateEntity();
                Ecb.AddComponent(entity, new PickupPerformedNotificationComponent());
            }
        }
    }
}
