using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.ResourceManagement.ResourceProviders;
using UnityEngine.SceneManagement;

public class TestAddressableSceneTwo : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void goBackToSceneOne()
    {
        AddressableManager.UnloadSceneAsync("secondScene");
        SceneManager.LoadScene(0);
    }
}
