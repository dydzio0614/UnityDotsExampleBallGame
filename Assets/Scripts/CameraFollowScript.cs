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
    private Vector3 _initialOffset;

    private void Awake()
    {
        _initialOffset = transform.position;
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
        transform.position = new Vector3(localTransform.Position.x + _initialOffset.x, _initialOffset.y, localTransform.Position.z + _initialOffset.z);
    }
}
