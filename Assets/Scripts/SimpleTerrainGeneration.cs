using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SimpleTerrainGeneration : MonoBehaviour
{
    public GameObject thisBlock;
    public float amp = 10f;
    public float freq = 10f;

    [SerializeField]
    private int chunkSize = 50;

    [SerializeField]
    private float noiseScale = .1f;

    [SerializeField, Range(0, 1)]
    private float threshold = 0.05f;

    [SerializeField]
    private Material material;

    [SerializeField]
    private bool sphere = false;

    private List<Mesh> meshes = new List<Mesh>();

    void Start()
    {
        generateTerrain();
        generatePlanet();

    }

    //convert perlin noise to perlin3D
    public static float Perlin3D(float x, float y, float z)
    {
        float ab = Mathf.PerlinNoise(x, y);
        float bc = Mathf.PerlinNoise(y, z);
        float ac = Mathf.PerlinNoise(x, z);

        float ba = Mathf.PerlinNoise(y, x);
        float cb = Mathf.PerlinNoise(z, y);
        float ca = Mathf.PerlinNoise(z, x);

        float abc = ab + bc + ac + ba + cb + ca;
        return abc / 6f;
    }

    //simple terrain generation method with perlin equation
    public void generateTerrain()
    {
        topGeneration(0,200,0);
        topGeneration(100,200,0);
        topGeneration(0, 200, 100);
        topGeneration(100, 200, 100);
        leftGeneration(200, 0, 0);
        leftGeneration(200, 0, 100);
        leftGeneration(200, 100, 0);
        leftGeneration(200, 100, 100);


        //generate underground

        /*        for (int x = 0; x < cols; x++)
                {
                    for (int z = 0; z < rows; z++)
                    {
                        //Apllying perlin noise to the y axis values
                        float y = Mathf.PerlinNoise(this.transform.position.x + x / freq, this.transform.position.z + z / freq) * amp;
                        y = Mathf.Floor(y);

                        GameObject newBlock = GameObject.Instantiate(thisBlock);

                        newBlock.transform.position = new Vector3(x, -y, z);


                    }
                }*/
    }
    public void topGeneration(int x_adjust, int y_adjust, int z_adjust)
    {
        int cols = 100;
        int rows = 100;
        int seed = Random.Range(0, 10000);
        float percentage = Random.Range(0.0f, 4.0f);
        List<CombineInstance> combine = new List<CombineInstance>();
        MeshFilter blockMesh = Instantiate(thisBlock, Vector3.zero, Quaternion.identity).GetComponent<MeshFilter>();
        int amp = Random.Range(5, 15);
        //coordinate Top(0,0)

        for (int x = 0; x < cols; x++)
        {
            for (int z = 0; z < rows; z++)
            {
                //Apllying perlin noise to the y axis values
                float y = Mathf.PerlinNoise(seed + this.transform.position.x + x / freq, this.transform.position.z + z / freq) * amp;
                y = Mathf.Floor(y);

                
                
                blockMesh.transform.position = new Vector3(x + x_adjust, y + y_adjust, z+ z_adjust);
                combine.Add(new CombineInstance
                {
                    mesh = blockMesh.sharedMesh,
                    transform = blockMesh.transform.localToWorldMatrix,
                });

                //Tree creation decision making
                if (Random.value * 100 < percentage)
                {
                    GameObject treePrimitive = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    treePrimitive.transform.position = new Vector3(x + x_adjust, y + 2 + y_adjust, z + z_adjust);
                    Vector3 treeVector = treePrimitive.transform.localScale;
                    treeVector.y = Random.value * 24;
                    treePrimitive.transform.localScale = treeVector;

                    GameObject treeHead = GameObject.CreatePrimitive(PrimitiveType.Cube);

                    treeHead.transform.localScale *= 4f;
                    treeHead.transform.position = new Vector3(x + x_adjust, y + 2 + y_adjust + treeVector.y / 2f, z + z_adjust);

                }
            }
        }
        Destroy(blockMesh.gameObject);
        List<List<CombineInstance>> blockDataLists = new List<List<CombineInstance>>();
        int vertexCount = 0;
        blockDataLists.Add(new List<CombineInstance>());
        for (int i = 0; i < combine.Count; i++)
        {
            vertexCount += combine[i].mesh.vertexCount;
            if (vertexCount > 65536)
            {
                vertexCount = 0;
                blockDataLists.Add(new List<CombineInstance>());
                i--;
            }
            else
            {
                blockDataLists.Last().Add(combine[i]);
            }
        }
        Transform container = new GameObject("Meshys").transform;
        foreach (List<CombineInstance> data in blockDataLists)
        {
            GameObject g = new GameObject("Meshy");
            g.transform.parent = container;
            MeshFilter mf = g.AddComponent<MeshFilter>();
            MeshRenderer mr = g.AddComponent<MeshRenderer>();
            mr.material = material;
            mf.mesh.CombineMeshes(data.ToArray());
            meshes.Add(mf.mesh);
            g.AddComponent<MeshCollider>().sharedMesh = mf.sharedMesh;
        }
    }

    public void leftGeneration(int x_adjust, int y_adjust, int z_adjust)
    {
        int cols = 100;
        int rows = 100;
        int seed = Random.Range(0, 10000);
        float percentage = Random.Range(0.0f, 4.0f);

        int amp = Random.Range(5, 15);
        //coordinate Top(0,0)

        for (int z = 0; z < cols; z++)
        {
            for (int y = 0; y < rows; y++)
            {
                //Apllying perlin noise to the y axis values
                float x = Mathf.PerlinNoise(seed + this.transform.position.z + z / freq, this.transform.position.y + y / freq) * amp;
                x = Mathf.Floor(x);

                GameObject newBlock = GameObject.Instantiate(thisBlock);
                
                newBlock.transform.position = new Vector3(x + x_adjust, y + y_adjust, z+ z_adjust);

                //Tree creation decision making
                if (Random.value * 100 < percentage)
                {
                    GameObject treePrimitive = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    treePrimitive.transform.position = new Vector3(x + 2 + x_adjust, y + y_adjust, z + z_adjust);
                    Vector3 treeVector = treePrimitive.transform.localScale;
                    treeVector.x = Random.value * 24;
                    treePrimitive.transform.localScale = treeVector;

                    GameObject treeHead = GameObject.CreatePrimitive(PrimitiveType.Cube);

                    treeHead.transform.localScale *= 4f;
                    treeHead.transform.position = new Vector3(x + 2 + x_adjust + treeVector.x / 2f, y + y_adjust, z + z_adjust);

                }
            }
        }
    }
    //generate the interior of the planet (needs more studying)
    public void generatePlanet()
    {
        int seed1 = Random.Range(0, 10000);
        int seed2 = Random.Range(0, 10000);
        float startTime = Time.realtimeSinceStartup;
        List<CombineInstance> blockData = new List<CombineInstance>();
        MeshFilter blockMesh = Instantiate(thisBlock, Vector3.zero, Quaternion.identity).GetComponent<MeshFilter>();

        for (int x = 0; x < chunkSize; x++)
        {
            for (int y = 0; y < chunkSize; y++)
            {
                for (int z = 0; z < chunkSize; z++)
                {
                    float noiseValue1 = Perlin3D(seed1 + x * noiseScale, y * noiseScale, z * noiseScale);
                    float noiseValue2 = Perlin3D(seed2 + x * noiseScale, y * noiseScale, z * noiseScale);

                    if (Mathf.Abs(noiseValue1) < threshold && Mathf.Abs(noiseValue2) < threshold)
                    {
                        float raduis = chunkSize / 2;
                        if (sphere && Vector3.Distance(new Vector3(x, y, z), Vector3.one * raduis) > raduis)
                            continue;
                        blockMesh.transform.position = new Vector3(x, y, z);
                        CombineInstance ci = new CombineInstance
                        {
                            mesh = blockMesh.sharedMesh,
                            transform = blockMesh.transform.localToWorldMatrix,
                        };
                        blockData.Add(ci);
                    }
                }

            }
        }
        Destroy(blockMesh.gameObject);
        //create lists and handle meshes as required (need further studying)
        List<List<CombineInstance>> blockDataLists = new List<List<CombineInstance>>();
        int vertexCount = 0;
        blockDataLists.Add(new List<CombineInstance>());
        for (int i = 0; i < blockData.Count; i++)
        {
            vertexCount += blockData[i].mesh.vertexCount;
            if (vertexCount > 65536)
            {
                vertexCount = 0;
                blockDataLists.Add(new List<CombineInstance>());
                i--;
            }
            else
            {
                blockDataLists.Last().Add(blockData[i]);
            }
        }
        Transform container = new GameObject("Meshys").transform;
        foreach (List<CombineInstance> data in blockDataLists)
        {
            GameObject g = new GameObject("Meshy");
            g.transform.parent = container;
            MeshFilter mf = g.AddComponent<MeshFilter>();
            MeshRenderer mr = g.AddComponent<MeshRenderer>();
            mr.material = material;
            mf.mesh.CombineMeshes(data.ToArray());
            meshes.Add(mf.mesh);
            g.AddComponent<MeshCollider>().sharedMesh = mf.sharedMesh;
        }
    }
}