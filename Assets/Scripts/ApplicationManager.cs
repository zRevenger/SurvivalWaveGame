using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApplicationManager : MonoBehaviour
{
    public static ApplicationManager instance;
    
    void Awake()
    {
        if (instance == null) instance = this;
    }

    private void Start()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
    }

    public void Quit(int code = 0) 
    {
        // Do required activities before exiting

        // If Editor, stop playing
        #if UNITY_EDITOR 
        UnityEditor.EditorApplication.isPlaying = false;
        #endif
        
        Application.Quit(code);
    }
}
