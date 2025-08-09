using Unity.Entities;
using UnityEngine;

class PlayerAuthoring : MonoBehaviour
{
    [field: SerializeField]
    public float PlayerSpeed { get; private set; } = 30f;
}

class PlayerBaker : Baker<PlayerAuthoring>
{
    public override void Bake(PlayerAuthoring authoring)
    {
        Entity entity = GetEntity(TransformUsageFlags.Dynamic);
        AddComponent(entity, new PlayerComponent { PlayerSpeed = authoring.PlayerSpeed } );
    }
}
