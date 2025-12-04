using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Clone_Skill : Skill
{
    [SerializeField] private GameObject clonePrefab;
    public bool canAttack { get; private set; } = true;
    public void CreatClone()
    {
        GameObject newClone=GameObject.Instantiate(clonePrefab,player.transform.position, Quaternion.identity);
    }
}
