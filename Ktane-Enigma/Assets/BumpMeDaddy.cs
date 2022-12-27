using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

[RequireComponent(typeof(KMMaterialInfo))]
public class BumpMeDaddy : MonoBehaviour {

    public Mesh Mesh;
    public Material[] Materials;

    private void Awake()
    {
        
        GetComponent<KMMaterialInfo>().ShaderNames = new string[] { "Standard", "Standard" }.ToList();
    }

    private IEnumerator Start()
    {
        yield return null;

        MeshRenderer renderer = gameObject.GetComponent<MeshRenderer>();
        renderer.sharedMaterials = Materials;
    }
}