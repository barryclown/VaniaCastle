using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordController : MonoBehaviour
{
    private Animator anim;
    private Rigidbody2D rb;
    private CircleCollider2D cd;
    private Player player;

    private bool canRoate=true;
    private bool isReturning;
    public bool isBouncing=false;
    public List<Transform> enemyTarget;
    private int tragetIndex;
    public int amountOfBounce=4;
    public int amountOfPierce;

    private float maxTravelDistance;
    private float spinDuration;
    private float spinTimer;
    private bool wasStopped;
    private bool isSpinning;
    private float spinDir;

    private void Awake()
    {
        
        rb = GetComponent<Rigidbody2D>();
        cd = GetComponent<CircleCollider2D>();
        anim = GetComponentInChildren<Animator>();
        player = PlayerManager.instance.player;
     
    }
    public void SetUpSword(Vector2 dir,float gravityScale)
    {
        
        rb.velocity = dir;
        spinDir = Mathf.Clamp(rb.velocity.x, -1, 1);
        rb.gravityScale = gravityScale;
        if(SkillManager.instance.Sword.swordType!=SwordType.Pierce)
        anim.SetBool("Rotation", true);

    }
    public void SetUpBounce(bool setUpBouncing)
    {
        isBouncing = setUpBouncing;
    }
    public void SetUpPierce(int setUpBouncing)
    {
        amountOfPierce=setUpBouncing;
    }
    public void SetUpSpin(bool setUpSpin,float _maxTravelDistance,float _spinDuration)
    {
        isSpinning = setUpSpin;
        maxTravelDistance= _maxTravelDistance;
        spinDuration= _spinDuration;
    }
    public void Update()
    {
        Debug.Log(spinDir);
        if (canRoate) 
        transform.right = rb.velocity;
        if (isReturning)
        {
            transform.position = Vector2.MoveTowards(transform.position, player.transform.position, Time.deltaTime * 10);
            if (Vector2.Distance(transform.position, player.transform.position) < 2)
                      player.ClearSword();
        }

        if (isBouncing && enemyTarget.Count > 0)
        {
            transform.position = Vector2.MoveTowards(transform.position, enemyTarget[tragetIndex].position, Time.deltaTime * 20);
            if (Vector2.Distance(transform.position, enemyTarget[tragetIndex].position) <= .1f)
            {
                if (enemyTarget[tragetIndex].transform.position.x > transform.position.x && enemyTarget[tragetIndex].GetComponent<Enemy>().facingDir > 0 || enemyTarget[tragetIndex].transform.position.x < transform.position.x && enemyTarget[tragetIndex].GetComponent<Enemy>().facingDir < 0)
                {
                    enemyTarget[tragetIndex].GetComponent<Enemy>().Damage(true);
                }
                else
                {
                    enemyTarget[tragetIndex].GetComponent<Enemy>().Damage(false);
                }
                tragetIndex++;
                amountOfBounce--;
                if (amountOfBounce < 0)
                {
                    isBouncing = false;
                    isReturning = true;
                }
                if (tragetIndex>=enemyTarget.Count)
                 {
                    tragetIndex = 0;
                }
            }
        }

        if (isSpinning)
        {
            if (Vector2.Distance(player.transform.position,transform.position)>maxTravelDistance&&!wasStopped)
            {
                StopWhenSpinning();
            }
            if (wasStopped)
            {
                spinTimer-= Time.deltaTime;
                transform.position = Vector2.MoveTowards(transform.position,new Vector2(transform.position.x+spinDir,transform.position.y),1.5f*Time.deltaTime);
                if (spinTimer < 0)
                {
                    isSpinning = false;
                    isReturning= true;
                }
            }

        }
    }

    private void StopWhenSpinning()
    {
        wasStopped = true;
        rb.constraints = RigidbodyConstraints2D.FreezePosition;
        spinTimer = spinDuration;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (isReturning)
            return;
        if (collision.GetComponent<Enemy>() != null)
        {
            Enemy enemy = collision.GetComponent<Enemy>();
            if (enemy.transform.position.x > transform.position.x && enemy.facingDir > 0 || enemy.transform.position.x < transform.position.x && enemy.facingDir < 0)
            {
                enemy.Damage(true);
            }
            else
            {
                enemy.Damage(false);
            }

        }
       
        if (collision.GetComponent<Enemy>() != null)
            if (isBouncing && enemyTarget.Count <= 0)
            {
                Collider2D[] colider = Physics2D.OverlapCircleAll(transform.position, 10);
                foreach (var item in colider)
                {
                    if (item.GetComponent<Enemy>() != null)
                        enemyTarget.Add(item.transform);
                }
            }
        if (amountOfPierce > 0 && collision.GetComponent<Enemy>() != null)
        {
            amountOfPierce--;
            return;
        }
        if (isSpinning)
        {
            StopWhenSpinning();
            return;
        }
         
        StuckInto(collision);
    }

    private void StuckInto(Collider2D collision)
    {

        rb.constraints = RigidbodyConstraints2D.FreezeAll;
        canRoate = false;
        cd.enabled = false;
        rb.isKinematic = true;
        //讓劍能卡在牆上
        if(isBouncing&&enemyTarget.Count>0)
            return ;
      
        anim.SetBool("Rotation", false);
        transform.parent = collision.transform;

    }
    public void ReturnSword()
    {
        transform.parent = null;      
        rb.constraints = RigidbodyConstraints2D.FreezeAll;
        isReturning = true;
       
    }
}
