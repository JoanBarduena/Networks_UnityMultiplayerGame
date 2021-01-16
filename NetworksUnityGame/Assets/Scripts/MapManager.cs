using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapManager : MonoBehaviour
{
    void Start()
    {
        // Spawn Boxes
        GameObject Boxes = GameObject.Find("Boxes");

        foreach (Transform child in Boxes.transform)
        {
            child.gameObject.GetComponent<BoxSpawner>().SpawnBox();
        }
    }
}
