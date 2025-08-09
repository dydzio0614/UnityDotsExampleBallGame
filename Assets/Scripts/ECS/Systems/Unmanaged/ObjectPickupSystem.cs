using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Physics;
using Unity.Physics.Systems;

[UpdateInGroup(typeof(FixedStepSimulationSystemGroup))]
[UpdateAfter(typeof(PhysicsSystemGroup))]
partial struct ObjectPickupSystem : ISystem
{
    [BurstCompile]
    public void OnCreate(ref SystemState state)
    {
        state.RequireForUpdate<SimulationSingleton>();
        state.RequireForUpdate<EndSimulationEntityCommandBufferSystem.Singleton>();
    }

    [BurstCompile]
    public void OnUpdate(ref SystemState state)
    {
        SimulationSingleton simulation = SystemAPI.GetSingleton<SimulationSingleton>();
        EntityCommandBuffer commandBufferSystem = SystemAPI.GetSingleton<EndSimulationEntityCommandBufferSystem.Singleton>()
            .CreateCommandBuffer(state.WorldUnmanaged);
        
        ComponentLookup<PickupObjectComponent> pickupObjectLookup = state.GetComponentLookup<PickupObjectComponent>(true);

        ObjectPickupTriggerEventsJob job = new ObjectPickupTriggerEventsJob()
        {
            PickupObjectsLookup = pickupObjectLookup,
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
            Entity objectToPickup = PickupObjectsLookup.EntityExists(triggerEvent.EntityA) ? triggerEvent.EntityA 
                : PickupObjectsLookup.EntityExists(triggerEvent.EntityB) ? triggerEvent.EntityB 
                : Entity.Null;

            if (objectToPickup != Entity.Null)
            {
                Ecb.SetEnabled(objectToPickup, false);
            }
        }
    }
}


