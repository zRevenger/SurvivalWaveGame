using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Steamworks;
using UnityEngine.UI;
using TMPro;

public class PlayerListItem : MonoBehaviour
{
    public string playerName;
    public int connectionID;
    public ulong playerSteamID;
    private bool avatarReceived;

    public TextMeshProUGUI playerNameText;
    public RawImage playerIcon;
    public Image hostIcon;

    public TextMeshProUGUI playerReadyText;
    public bool ready;

    protected Callback<AvatarImageLoaded_t> imageLoadedCallback;

    public void ChangeReadyStatus()
    {
        if(ready)
        {
            playerReadyText.text = "Ready";
            playerReadyText.color = Color.green;
        }
        else
        {
            playerReadyText.text = "Not Ready";
            playerReadyText.color = Color.red;
        }
    }

    private void Start()
    {
        imageLoadedCallback = Callback<AvatarImageLoaded_t>.Create(OnImageLoaded);
    }

    private void OnImageLoaded(AvatarImageLoaded_t callback)
    {
        if (callback.m_steamID.m_SteamID == playerSteamID)
            playerIcon.texture = GetSteamImageAsTexture(callback.m_iImage);
        else
            return;
    }

    public void SetPlayerValues(bool isHost)
    {
        playerNameText.text = playerName;
        ChangeReadyStatus();
        if (!avatarReceived)
            GetPlayerIcon();

        if (isHost)
            hostIcon.gameObject.SetActive(true);
    }

    void GetPlayerIcon()
    {
        int ImageID = SteamFriends.GetLargeFriendAvatar((CSteamID)playerSteamID);
        //-1 = error
        if (ImageID == -1) return;
        playerIcon.texture = GetSteamImageAsTexture(ImageID);
    }
    private Texture2D GetSteamImageAsTexture(int iImage)
    {
        Texture2D texture = null;

        bool isValid = SteamUtils.GetImageSize(iImage, out uint width, out uint height);
        if (isValid)
        {
            byte[] image = new byte[width * height * 4];

            isValid = SteamUtils.GetImageRGBA(iImage, image, (int)(width * height * 4));

            if(isValid)
            {
                texture = new Texture2D((int)width, (int)height, TextureFormat.RGBA32, false, true);
                texture.LoadRawTextureData(image);
                texture.Apply();
            }
        }
        avatarReceived = true;
        return texture;
    }
}