using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;

public class PlayerClassController : NetworkBehaviour
{
    public GameObject[] playerModels;

    //public MeshRenderer mesh;
    public PlayableClass[] classes;

    private PlayerObjectController playerObjectController;

    private void Start()
    {
        playerObjectController = GetComponent<PlayerObjectController>();
    }

    private bool hasInitialized;

    private void Update()
    {
        if (playerObjectController.IsInGameScene())
        {
            if (!hasInitialized )
            {
                PlayerClassModelSetup();
                hasInitialized = true;
            }

            if (!hasAuthority) return;
        }
    }

    public void PlayerClassModelSetup()
    {
        for(int i = 0; i < classes.Length; i++)
        {
            if(playerObjectController.playerClass == i)
            {
                playerModels[i].SetActive(true);
                playerObjectController.playerModel = playerModels[i];
                playerObjectController.animator = playerModels[i].GetComponent<Animator>();
                if (hasAuthority)
                {
                    playerObjectController.playerModel.transform.GetChild(0).gameObject.SetActive(false);
                    playerObjectController.playerModel.transform.GetChild(1).gameObject.SetActive(false);
                }
                break;
            }
        }
        //mesh.material = classes[GetComponent<PlayerObjectController>().playerClass].tempColor;
    }
}
