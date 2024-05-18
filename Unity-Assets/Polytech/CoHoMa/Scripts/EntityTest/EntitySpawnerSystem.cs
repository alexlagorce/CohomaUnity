using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
using UnityEngine;
using Random = Unity.Mathematics.Random;
using Unity.Rendering;

public class EntitySpawnerSystem : ComponentSystem
{
    private float spawnTimer;
    private Random random;
    protected override void OnCreate()
    {
        random = new Random(56);
    }

    protected override void OnUpdate()
    {
        spawnTimer -= Time.DeltaTime;

        if (spawnTimer <= 0f)
        {
            spawnTimer = 2f;

            /*
            //Spawn entities
            Entities.ForEach((ref PrefabEntityComponent prefabEntityComponent) => {
                Entity spawnedEntity = EntityManager.Instantiate(prefabEntityComponent.prefabEntity);

                float3 position = new float3(random.NextFloat(-5f, 5f), random.NextFloat(-5f, 5f), 0);
                EntityManager.SetComponentData(spawnedEntity, new Translation { Value = position });

                Debug.Log($"Spawned entity at position: {position}");
            });
            */

            /* Other method
            Entity spawnedEntity = EntityManager.Instantiate(PrefabEntities.prefabEntity);

            float3 position = new float3(random.NextFloat(-5f, 5f), random.NextFloat(-5f, 5f), 0);
            EntityManager.SetComponentData(spawnedEntity, new Translation { Value = position });

            Debug.Log($"Spawned entity at position: {position}");
            */
            
        }
    }
}

