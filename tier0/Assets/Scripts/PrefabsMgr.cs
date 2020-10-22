using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrefabsMgr : MonoBehaviour
{
    //TODO: move to ScriptableObjects

    // holder for prefabs, before we use ScriptableObjects...
    public GameObject[] PrefabBlocks;
    public GameObject blockPrefab;
    public Material[] blockMaterials;


    public static PrefabsMgr instance;
    // Start is called before the first frame update
    void Start()
    {
        instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
