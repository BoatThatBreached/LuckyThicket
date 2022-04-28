using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class CustomSettings : MonoBehaviour
{
    // Start is called before the first frame update
    public void Return()
    {
        SceneManager.LoadScene("MenuScene");
    }
}
