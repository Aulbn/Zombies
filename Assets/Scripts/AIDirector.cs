﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIDirector : MonoBehaviour
{
    public static AIDirector Instance;

    private Dictionary<PlayerController, bool> inCombat;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;

        inCombat = new Dictionary<PlayerController, bool>();
    }

    public static void SetInCombat(PlayerController player, bool inCombat)
    {
        if (!Instance.inCombat.ContainsKey(player)) Instance.inCombat.Add(player, false);
        if (Instance.inCombat[player]) return;

        Instance.inCombat[player] = inCombat;
        //Start wave!
    }

    private IEnumerator Wave(int zombiesToSpawn)
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
                //Spawn
                //set new values
                zombiesLeft--;
            }
            yield return null;
        }

    }

}
