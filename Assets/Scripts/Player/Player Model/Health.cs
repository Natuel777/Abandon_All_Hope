using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Health : IDamageable
{
    public float MaxLife {get; private set;}
    public float Life {get; private set;}
    
    public Health(float maxLife)
    {
        MaxLife = maxLife;

        RestoreHealth();
        EventManager.Subscribe("OnPlayerDamaged", TakeDamage);
    }

    
    public void ReceiveDamageValue(float dmg)
    {
        EventManager.Trigger("OnPlayerDamaged", false, new DamageData(dmg, Life), UnityEngine.Random.Range(0.1f, 1f));
    }

    //Preguntar si es conveniente usar object, demasiado uso de memoria?
    //El garbage collector no lo compensa?
    public void TakeDamage(params object[] parameters)
    {
        foreach(var item in parameters)
        {
            if(item is DamageData damageData)
            {
                Life -= damageData.Damage;
                break;
            }
        }

        if(Life <= 0) EventManager.Trigger("OnPlayerDeath", true, true);

        Debug.Log($"vida actual: {Life}");
    }

    public void RestoreHealth()
    {
        Life = MaxLife;
    }

    public void Dispose()
    {
        EventManager.Unsubscribe("OnPlayerDamaged", TakeDamage);
    }

    public void SetHealth(float h)
    {
        Life = h;
    }
}

public struct DamageData
{
    public float Damage {get; private set;}
    public float CurrentHealth {get; private set;}

    public DamageData(float dmg, float life)
    {
        Damage = dmg;
        CurrentHealth = life;
    }
}

