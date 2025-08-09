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
        AddComponent<SpinningRotationComponent>(entity, new SpinningRotationComponent() { AngleValuePerSecond = 10f });
    }
}
