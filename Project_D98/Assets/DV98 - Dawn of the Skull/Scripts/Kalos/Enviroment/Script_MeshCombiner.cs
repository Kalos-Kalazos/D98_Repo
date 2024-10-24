using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif



public class Script_MeshCombiner : MonoBehaviour
{
    [SerializeField] private List<MeshFilter> listMeshFilter;

    [SerializeField] private MeshFilter TargetMesh;

    [ContextMenu("Combine Meshes")]

    private void CombineMesh()
    {        
       CombineInstance[] combine = new CombineInstance[listMeshFilter.Count];

        for (int i = 0; i < listMeshFilter.Count; i++)
        {
            combine[i].mesh = listMeshFilter[i].sharedMesh;
            combine[i].transform = listMeshFilter[i].transform.localToWorldMatrix;
            listMeshFilter[i].gameObject.SetActive(false);
        }

        

        var mesh = new Mesh(); 
        mesh.CombineMeshes(combine);
        mesh.RecalculateBounds();
        mesh.RecalculateNormals();
        TargetMesh.mesh = mesh;

        #if UNITY_EDITOR
             SaveMesh(TargetMesh.sharedMesh, gameObject.name, false, true); 
        #endif
    }

#if UNITY_EDITOR
    public static void SaveMesh(Mesh mesh, string name, bool makeNewInstance, bool optimizeMesh)
    {
        string path = EditorUtility.SaveFilePanel("Save Separate Mesh Asset", "Assets/", name, "asset");
        if (string.IsNullOrEmpty(path)) return;

        path = FileUtil.GetProjectRelativePath(path);

        Mesh meshToSave = (makeNewInstance) ? Object.Instantiate(mesh) as Mesh : mesh;

        if(optimizeMesh) MeshUtility.Optimize(meshToSave);

        AssetDatabase.CreateAsset(meshToSave, path);
        AssetDatabase.SaveAssets();
    }
#endif

    //void CombineMesh()
    // {
    //    CombineInstance[] combine = new CombineInstance[listMeshFilter.Count];
    //
    //    for (int i = 0; i < listMeshFilter.Count; i++)
    //     {
    //         combine[i].mesh = listMeshFilter[i].sharedMesh;
    //         combine[i].transform = listMeshFilter[i].transform.localToWorldMatrix;
    //         listMeshFilter[i].gameObject.SetActive(false);
    //     }
    //
    //     var meshFilter = transform.GetComponent<MeshFilter>();
    //     meshFilter.mesh = new Mesh();
    //     meshFilter.mesh.CombineMeshes(combine);
    //
    //       meshFilter.mesh.RecalculateBounds();
    //
    //      meshFilter.mesh.RecalculateNormals();
    //
    //       transform.GetComponent<MeshFilter>().sharedMesh = transform.GetComponent<MeshFilter>().mesh;
    //     GetComponent<MeshFilter>().sharedMesh = meshFilter.mesh;
    //      transform.gameObject.SetActive(true);
    //
    //    transform.rotation = Quaternion.identity;
    //    transform.position = Vector3.zero;
    //}
}
