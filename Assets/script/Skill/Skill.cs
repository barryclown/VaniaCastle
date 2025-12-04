using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Skill :MonoBehaviour
{
    [SerializeField] protected float coldown;
    protected float coldownTimer;
    protected Player player;
    protected virtual void Start()
    {
        player=PlayerManager.instance.player;
    }
    protected virtual void Update()
    {
        coldownTimer-=Time.deltaTime;
    }
    public virtual bool CheckSkill()
    {
        if (coldownTimer<=0)
        {
            UseSkill();
            coldownTimer = coldown;
            return true;
        }
        return false;
    }
    public virtual void UseSkill()
    {

    }
}
