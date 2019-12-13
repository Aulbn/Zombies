using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    public GameObject debugTools;
    public Text timerText;

    [SerializeField] private GameObject playerUIPrefab;
    public static GameObject PlayerUIPrefab { get { return Instance.playerUIPrefab; } }


    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Backslash)) //Toggle debug tools
            debugTools.SetActive(!debugTools.activeSelf);
    }

    public static void SetTimerText(float time)
    {
        if (!Instance.debugTools.activeSelf) return;

        int min = Mathf.FloorToInt(time / 60);
        int sec = Mathf.FloorToInt(time % 60);
        Instance.timerText.text = min.ToString("00") + ":" + sec.ToString("00");
    }

    public static void SetMultiCameraLayout()
    {
        int playerCount = PlayerController.AllPlayers.Count;
        switch (playerCount)
        {
            case 1:
                //Player 1
                //PlayerController.AllPlayers[0].cam.rect = new Rect(0,0,1,1);
                PlayerController.AllPlayers[0].SetCameraRect(0, 0, 1, 1);
                ((RectTransform)PlayerController.AllPlayers[0].PlayerUI.transform).anchorMin = new Vector2(0, 0);sadaaadawada
                ((RectTransform)PlayerController.AllPlayers[0].PlayerUI.transform).anchorMax = new Vector2(1, 1);
                break;
            case 2:
                //Player 1
                PlayerController.AllPlayers[0].cam.rect = new Rect(0, 0.5f, 1, 0.5f);
                ((RectTransform)PlayerController.AllPlayers[0].PlayerUI.transform).anchorMin = new Vector2(0, 0.5f);
                ((RectTransform)PlayerController.AllPlayers[0].PlayerUI.transform).anchorMax = new Vector2(1, 1);
                //Player 2
                PlayerController.AllPlayers[1].cam.rect = new Rect(0, 0, 1, 0.5f);
                ((RectTransform)PlayerController.AllPlayers[1].PlayerUI.transform).anchorMax = new Vector2(1, 0.5f);
                ((RectTransform)PlayerController.AllPlayers[0].PlayerUI.transform).anchorMax = new Vector2(1, 1);
                break;
            case 3:
                //Player 1
                PlayerController.AllPlayers[0].cam.rect = new Rect(0, 0.5f, 1, 0.5f);
                ((RectTransform)PlayerController.AllPlayers[0].PlayerUI.transform).anchorMin = new Vector2(0, 0.5f);
                ((RectTransform)PlayerController.AllPlayers[0].PlayerUI.transform).anchorMax = new Vector2(1, 1);
                //Player 2
                PlayerController.AllPlayers[1].cam.rect = new Rect(0, 0, 0.5f, 0.5f);
                ((RectTransform)PlayerController.AllPlayers[1].PlayerUI.transform).anchorMin = new Vector2(0, 0);
                ((RectTransform)PlayerController.AllPlayers[1].PlayerUI.transform).anchorMax = new Vector2(0.5f, 0.5f);
                //Player 3
                PlayerController.AllPlayers[2].cam.rect = new Rect(0.5f, 0, 0.5f, 0.5f);
                ((RectTransform)PlayerController.AllPlayers[2].PlayerUI.transform).anchorMin = new Vector2(0.5f, 0);
                ((RectTransform)PlayerController.AllPlayers[2].PlayerUI.transform).anchorMax = new Vector2(1, 0.5f);
                break;
            case 4:
                //Player 1
                PlayerController.AllPlayers[0].cam.rect = new Rect(0, 0.5f, 0.5f, 0.5f);
                ((RectTransform)PlayerController.AllPlayers[0].PlayerUI.transform).anchorMin = new Vector2(0, 0.5f);
                ((RectTransform)PlayerController.AllPlayers[0].PlayerUI.transform).anchorMax = new Vector2(0.5f, 1);
                //Player 2
                PlayerController.AllPlayers[1].cam.rect = new Rect(0.5f, 0.5f, 0.5f, 0.5f);
                ((RectTransform)PlayerController.AllPlayers[1].PlayerUI.transform).anchorMin = new Vector2(0.5f, 0.5f);
                ((RectTransform)PlayerController.AllPlayers[1].PlayerUI.transform).anchorMax = new Vector2(1, 1);
                //Player 3
                PlayerController.AllPlayers[2].cam.rect = new Rect(0, 0, 0.5f, 0.5f);
                ((RectTransform)PlayerController.AllPlayers[1].PlayerUI.transform).anchorMin = new Vector2(0, 0);
                ((RectTransform)PlayerController.AllPlayers[1].PlayerUI.transform).anchorMax = new Vector2(0.5f, 0.5f);
                //Player 4
                PlayerController.AllPlayers[2].cam.rect = new Rect(0.5f, 0, 0.5f, 0.5f);
                ((RectTransform)PlayerController.AllPlayers[2].PlayerUI.transform).anchorMin = new Vector2(0.5f, 0);
                ((RectTransform)PlayerController.AllPlayers[2].PlayerUI.transform).anchorMax = new Vector2(1, 0.5f);
                break;
            default:
                break;

        }
    }
}
