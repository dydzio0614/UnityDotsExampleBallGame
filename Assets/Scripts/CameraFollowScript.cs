using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using UnityEngine;

public class CameraFollowScript : MonoBehaviour
{
    private EntityManager _entityManager;
    private EntityQuery _playerQuery;
    private Entity _target;

    private void Awake()
    {
        _entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;

        _playerQuery = _entityManager
            .CreateEntityQuery(ComponentType.ReadOnly<PlayerComponent>(), ComponentType.ReadOnly<LocalTransform>());
    }

    private void Start()
    {
        NativeArray<Entity> entities = _playerQuery.ToEntityArray(Allocator.Temp);
        
        if (entities.Length == 0)
            Debug.LogError("No player entity found!");
        
        _target = entities[0];
        
        entities.Dispose();
    }

    private void LateUpdate()
    {
        LocalTransform localTransform = _entityManager.GetComponentData<LocalTransform>(_target);
        transform.position = new Vector3(localTransform.Position.x, 13.5f, localTransform.Position.z - 15f);
    }
}
