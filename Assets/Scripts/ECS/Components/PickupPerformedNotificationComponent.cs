using Unity.Entities;

namespace ECS.Components
{
    //this component exists to make PickupActivationSystem logic reactive, rather than polling entities every update cycle
    public struct PickupPerformedNotificationComponent : IComponentData
    {
    }
}