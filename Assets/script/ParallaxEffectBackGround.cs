using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallaxEffectBackGround : MonoBehaviour
{
    private GameObject cam;
    [SerializeField]private float parallaxEffect;
    private float xPosition;
    private float length;
    public SpriteRenderer sr;
    // Start is called before the first frame update
    void Start()
    {
        cam = GameObject.Find("Main Camera");
        xPosition = transform.position.x;
        length=sr.size.x;
    }

    // Update is called once per frame
    void Update()
    {
        float distanaceMove = cam.transform.position.x * (1-parallaxEffect);
        float distanaceToMove = cam.transform.position.x * parallaxEffect;

        transform.position = new Vector3(xPosition + distanaceToMove, transform.position.y);

        if (distanaceMove > xPosition+length )
            xPosition=xPosition+length;
        if (distanaceMove < xPosition - length)
            xPosition = xPosition - length;




    }
}
