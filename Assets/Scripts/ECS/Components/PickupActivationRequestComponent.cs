using Unity.Entities;

//this component exists to allow making PickupActivationSystem logic reactive, rather than polling entities every update cycle
public struct PickupActivationRequestComponent : IComponentData
{
}