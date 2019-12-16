using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.UI;
using UnityEngine.UI;
using TMPro;


public class PlayerUI : MonoBehaviour
{
    [HideInInspector] public MultiplayerEventSystem eventSystem;
    private PlayerController player;

    public Image crosshair;
    public GameObject menu;
    public Selectable firstMenuItem;

    [Header("HUD")]
    [SerializeField] private TextMeshProUGUI ammoText;
    [SerializeField] private TextMeshProUGUI magText;
    [SerializeField] private Image healthbar;
    //[SerializeField] private Image firstWeapon, secondWeapon, ammoMeter;

    private void Update()
    {
        healthbar.fillAmount = (float)player.health / 100;
    }

    public void SetUp(PlayerController player, MultiplayerEventSystem eventSystem)
    {
        this.player = player;
        this.eventSystem = eventSystem;
        eventSystem.playerRoot = gameObject;
    }

    public void SetAmmoText(WeaponHandler weaponHandler)
    {
        if (!player.ammoStash.ContainsKey(weaponHandler.weapon.ammoType)) return;
        SetAmmoText(weaponHandler.currentAmmo, player.ammoStash[weaponHandler.weapon.ammoType]);
    }
    public void SetAmmoText(int currentAmmo, int maxAmmo)
    {
        ammoText.text = "" + currentAmmo;
        magText.text =  "/" + maxAmmo;
        //ammoMeter.fillAmount = (float)(maxAmmo - currentAmmo) / maxAmmo;
    }

    public void ToggleMenu()
    {
        menu.SetActive(!menu.activeSelf);
        eventSystem.SetSelectedGameObject(menu.activeSelf ? firstMenuItem.gameObject : null); 
    }

    public void SetCrosshairOpacity(float opacity)
    {
        Color color = crosshair.color;
        color.a = opacity;
        crosshair.color = color;
    }

    public void DisconnectPlayer()
    {
        player.Disconnect();
        Destroy(gameObject);
    }

}
