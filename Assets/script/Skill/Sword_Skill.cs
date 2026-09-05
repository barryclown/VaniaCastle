using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum SwordType
{
    Regular,
    Bounce,
    Pierce,
    Spin
}

public class Sword_Skill : Skill
{
    public SwordType swordType;
    [SerializeField] private GameObject swordPerfab;
    [SerializeField] public Vector2 lanchForce;
    [SerializeField] private float swordGravity;
    public Vector2 finalDir;
    [SerializeField] private int numberOfDots;
    [SerializeField] public float spaceBetweenDots;
    [SerializeField] private GameObject dotsPerfab;
    [SerializeField] private Transform dotsParent;
    public GameObject[] dots;
    private Camera cam;
    protected override void Start()
    {
        base.Start();
        cam = Camera.main;
        GenerateDots();
    }
    protected override void Update()
    {


    }
    public void SetSwordGravity()
    {
        switch (swordType)
        {
            case SwordType.Bounce:
                swordGravity = 4f;
                break;

            case SwordType.Pierce:
                swordGravity = 0.1f;
                break;

            case SwordType.Spin:
                swordGravity = 2f;
                break;

            default:
                swordGravity = 1f; // Regular
                break;
        }
    }
    public void CreatSword()
    {
        GameObject newSword = Instantiate(swordPerfab, player.transform.position, transform.rotation);

        if (swordType == SwordType.Bounce)
        {

            newSword.GetComponent<SwordController>().SetUpBounce(true);
        }
        else if (swordType == SwordType.Pierce)
        {
            newSword.GetComponent<SwordController>().SetUpPierce(2);
        }
        else if (swordType == SwordType.Spin)
        {
            newSword.GetComponent<SwordController>().SetUpSpin(true, 7, 2);
        }
        newSword.GetComponent<SwordController>().SetUpSword(finalDir, swordGravity);
        player.AssignNewSword(newSword);
        SkillManager.instance.Sword.DotsActive(false);
    }
    public Vector2 AimDirection()
    {
        if (cam == null) cam = Camera.main;
        return player.transform.position - cam.ScreenToWorldPoint(Input.mousePosition);
    }
    public void DotsActive(bool isActive)
    {
        for (int i = 0; i < dots.Length; i++)
        {
            dots[i].SetActive(isActive);
        }
    }
    private void GenerateDots()
    {
        dots = new GameObject[numberOfDots];
        for (int i = 0; i < numberOfDots; i++)
        {
            dots[i] = Instantiate(dotsPerfab, player.transform.position, Quaternion.identity, dotsParent);
            dots[i].SetActive(false);
        }
    }
    public Vector2 DotsPosition(float t)
    {
        return (Vector2)player.transform.position +
        new Vector2(AimDirection().normalized.x * lanchForce.x,
        //���z������ ���O�[�t��
        AimDirection().normalized.y * lanchForce.y) * t + 0.5f * Physics2D.gravity * swordGravity * t * t;
    }
    public void SwitchSwordType()
    {
        // ���o�ثe�Ҧ����������
        int current = (int)swordType;

        // enum �ƶq
        int total = System.Enum.GetNames(typeof(SwordType)).Length;

        // �U�@�ӡ]�W�L�N�^ 0�^
        current = (current + 1) % total;

        // �]�w�s�Ҧ�
        swordType = (SwordType)current;

        // ��s���O
        SetSwordGravity();
    }

}
