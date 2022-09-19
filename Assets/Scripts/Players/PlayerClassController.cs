using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;

public class PlayerClassController : NetworkBehaviour
{
    public GameObject playerModel;

    public MeshRenderer mesh;
    public PlayableClass[] classes;

    private void Start()
    {
        playerModel.SetActive(false);
    }

    private bool hasInitialized;

    private void Update()
    {
        if (GetComponent<PlayerObjectController>().IsInGameScene())
        {
            if (!hasInitialized && GetComponent<PlayerObjectController>().playerModel.activeSelf)
            {
                PlayerClassModelSetup();
                hasInitialized = true;
            }

            if (!hasAuthority) return;
        }
    }

    public void PlayerClassModelSetup()
    {
        mesh.material = classes[GetComponent<PlayerObjectController>().playerClass].tempColor;
    }
}
