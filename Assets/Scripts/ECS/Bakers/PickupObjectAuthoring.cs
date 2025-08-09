using Unity.Entities;
using UnityEngine;

class PickupObjectAuthoring : MonoBehaviour
{

}

class PickupObjectBaker : Baker<PickupObjectAuthoring>
{
    public override void Bake(PickupObjectAuthoring authoring)
    {
        Entity entity = GetEntity(TransformUsageFlags.Dynamic);
        
        AddComponent<PickupObjectComponent>(entity);
        AddComponent(entity, new SpinningRotationComponent() { DegreesPerSecond = 20f });
        AddComponent<Disabled>(entity);
        
        
    }
}
