using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using UnityEngine;
using Random = Unity.Mathematics.Random;
using Unity.Rendering;

public class SingleEntitySpawnerSystem : ComponentSystem
{
    private bool entitySpawned = false;
    private Random random;
    protected override void OnCreate()
    {
        random = new Random(56);
    }

    protected override void OnUpdate()
    {
        if (!entitySpawned)
        {
            //Spawn entities
            Entities.ForEach((ref PrefabEntityComponent prefabEntityComponent) => {
                Entity spawnedEntity = EntityManager.Instantiate(prefabEntityComponent.prefabEntity);

                float3 position = new float3(0, 0, 0);
                EntityManager.SetComponentData(spawnedEntity, new Translation { Value = position });

                Debug.Log($"Spawned entity at position: {position}");

                entitySpawned = true;
            });
        }
    }
}

