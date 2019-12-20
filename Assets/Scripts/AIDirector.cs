using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIDirector : MonoBehaviour
{
    public static AIDirector Instance;

    private Dictionary<PlayerController, CombatInfo> PlayerCombatInfo;

    struct CombatInfo
    {
        public bool InCombat { get; private set; }
        public IEnumerator WaveCoroutine { get; private set; }

        public void SetCombat(bool inCombat)
        {
            InCombat = inCombat;
        }
        public void StartCombat(IEnumerator coroutine)
        {
            if (WaveCoroutine != null)
            WaveCoroutine = coroutine;
        }
    }

    private void Awake()
    {
        if (Instance == null)
            Instance = this;

        PlayerCombatInfo = new Dictionary<PlayerController, CombatInfo>();
    }

    public static void SetInCombat(PlayerController player, bool inCombat)
    {
        if (!Instance.PlayerCombatInfo.ContainsKey(player)) Instance.PlayerCombatInfo.Add(player, new CombatInfo());
        if (Instance.PlayerCombatInfo[player].InCombat) return;

        Instance.PlayerCombatInfo[player].SetCombat(inCombat);
        //Start wave!
    }

    public void InitiateCombat(PlayerController player)
    {
        
    }

    private IEnumerator Wave(int zombiesToSpawn, PlayerController player)
    {
        int zombiesLeft = zombiesToSpawn;
        float interval = 0;
        float timer = 0;

        while (zombiesLeft > 0)
        {
            if (timer < interval)
            {
                timer += Time.deltaTime;
            }
            else
            {
                //Find spawner
                //player
                //Spawn
                //set new values
                zombiesLeft--;
            }
            yield return null;
        }

    }

}
