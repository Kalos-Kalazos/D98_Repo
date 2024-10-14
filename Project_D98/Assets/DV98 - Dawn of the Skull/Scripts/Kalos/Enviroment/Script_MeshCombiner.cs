using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]

public class Script_MeshCombiner : MonoBehaviour
{

    void Start()
    {

    }

    void Update()
    {
        CombineMesh();
    }

    void CombineMesh()
    {

        MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>();
        CombineInstance[] combine = new CombineInstance[meshFilters.Length];

        int i = 0;
        while (i < meshFilters.Length)
        {
            combine[i].mesh = meshFilters[i].sharedMesh;
            combine[i].transform = meshFilters[i].transform.localToWorldMatrix;
            meshFilters[i].gameObject.SetActive(false);

            i++;
        }

        var meshFilter = transform.GetComponent<MeshFilter>();
        meshFilter.mesh = new Mesh();
        meshFilter.mesh.CombineMeshes(combine);
        transform.GetComponent<MeshCollider>().sharedMesh = transform.GetComponent<MeshFilter>().mesh;
        GetComponent<MeshFilter>().sharedMesh = meshFilter.mesh;
        transform.gameObject.SetActive(true);

        transform.rotation = Quaternion.identity;
        transform.position = Vector3.zero;
    }
}
