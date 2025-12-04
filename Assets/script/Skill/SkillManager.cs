using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillManager : MonoBehaviour
{
   public static SkillManager instance;
    public Dash_Skill Dash{get; private set;}
    public Clone_Skill Clone{ get; private set; }
    public Sword_Skill Sword{get; private set;}
    public void Awake()
    {
        if(instance!=null)
            Destroy(instance);
        else
        instance = this;
    }
    public void Start()
    {
        Dash = GetComponent<Dash_Skill>();
        Clone = GetComponent<Clone_Skill>();
        Sword = GetComponent<Sword_Skill>();
    }
}
