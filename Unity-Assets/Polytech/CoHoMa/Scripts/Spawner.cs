using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    private SpawnData[] spawnArray = new SpawnData[10000];
    private float delay  = 2f;
    private float currentTime = 0f;
    public  GameObject cube;

    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < spawnArray.Length; i++)
        {
            spawnArray[i] = new SpawnData();
            spawnArray[i].x = Random.Range(0, 10);
            spawnArray[i].y = Random.Range(0, 10);
            spawnArray[i].z = Random.Range(0, 10);
            spawnArray[i].color_r = Random.Range(0f, 1f);
            spawnArray[i].color_g = Random.Range(0f, 1f);
            spawnArray[i].color_b = Random.Range(0f, 1f);
            spawnArray[i].position = new Vector3(spawnArray[i].x, spawnArray[i].y, spawnArray[i].z);
        }

        foreach (var spawnData in spawnArray)
        {
            Debug.Log($"Position: {spawnData.position}");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(currentTime + delay < Time.fixedTime){

            for (int i = 0; i < spawnArray.Length; i++)
            {
                var tempCube = Instantiate(cube, spawnArray[i].position, Quaternion.identity);

                Renderer renderer = tempCube.GetComponent<Renderer>();
                renderer.material.SetColor("_Color", new Color(spawnArray[i].color_r, spawnArray[i].color_g, spawnArray[i].color_b, 1f));

                currentTime = Time.fixedTime;
                Destroy(tempCube, 0.5f);
            }
        }
    }
}
