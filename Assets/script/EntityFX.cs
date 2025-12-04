using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntityFX : MonoBehaviour
{
    public SpriteRenderer sr;
    [SerializeField] private Material hitmat;
    [SerializeField] private Material yellowmat;
    private Material originaMat;

    private void Start()
    {
        sr = GetComponentInChildren<SpriteRenderer>();
        originaMat = sr.material;
    }
    
    private IEnumerator FlashWhiteFX()
    {
   
        sr.material = hitmat;
        yield return new WaitForSeconds(.2f);
        sr.material = originaMat;
    }
    private IEnumerator FlashYellowFX()
    {

        sr.material = yellowmat;
        yield return new WaitForSeconds(.2f);
        sr.material = originaMat;
    }
    private void RedColorBlink()
    {
        if (sr.color !=Color.white )
            sr.color = Color.white;
        else
            sr.color = Color.red;
    }
    private void CancelRedBlink()
    {
        CancelInvoke();
        sr.color = Color.white;
    }



}
