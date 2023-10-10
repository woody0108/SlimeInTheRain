using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ASDF : MonoBehaviour
{
    // Start is called before the first frame update
    SceneDesign sceneDesign;
    JellyManager jellyManager;
    private void Start()
    {
        sceneDesign = SceneDesign.Instance;
        jellyManager = JellyManager.Instance;
    }
    public void MapClear()
    {
        sceneDesign.mapClear = true;
    }
    public void Jelly100()
    {
        jellyManager.JellyCount += 100;
    }
}
