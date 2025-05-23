using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    Rigidbody2D rbody;
    float axisH = 0.0f;
    public float speed = 3.0f; // �ӵ��� public���� �����ؼ� �ν����Ϳ��� ������ �� �ֵ��� ��

    public float jump = 9.0f;// ������
    public LayerMask groundLayer; // ������ �� �ִ� ���̾�
    bool goJump = false; // ���� ���� �÷���
    bool onGround = false; // �ٴڿ� �ִ��� üũ�ϴ� �÷���

    // Start is called before the first frame update
    void Start()
    {
        rbody= this.GetComponent<Rigidbody2D>(); // ó������ �ѹ� �ҷ����ϱ� start�� ����
    }

    // Update is called once per frame
    // update�� ������ ȣ��Ǵϱ� �� �ȿ� �÷��̾��� ���¸� ����üũ�ϴ� �ڵ带 �־� ����Ѵ�.
    // �׻� ���Ǻ��̴� ������ ������.
    void Update()
    {
        axisH = Input.GetAxisRaw("Horizontal");// Ű�� ���������� ���ȴ����� ��� Ȯ�� / �ƹ��͵��ȴ�����0, ����Ű��-1, ������Ű��1
        // ���� ����
        if ( axisH > 0.0f) // 0�϶� �̰� ������ xxx ex)2m������ ������ ������ > 2���� ũ��, 2���������� �̷��� ��(��Ȯ�� ���ڰ� �� ������ ����) ��ȣ�� ���� ����!!!
        {
            // ������ �̵�
            Debug.Log("������ �̵�"); // ����׷� Ȯ��
            transform.localScale = new Vector2(1, 1); // ���� this�� �پ����� . �÷��̾� ��ü�� transform�� localScale�� ������.(�θ��� ���� �������) 
        }
        else if ( axisH < 0.0f)
        {
            // ���� �̵�
            Debug.Log("���� �̵�"); // ����׷� Ȯ��
            transform.localScale = new Vector2(-1, 1);  // �¿����
        }

        // ĳ���� ����
        if( Input.GetButtonDown("Jump"))
        {
            Jump();// ����
        }
        
    }

    // 
    void FixedUpdate()
    {
        // ���� ����
        onGround = Physics2D.Linecast(transform.position,
                                      transform.position - (transform.up * 0.1f), // 0.1f�� �ٴڰ��� �Ÿ�
                                      groundLayer); // �������� ��� �Ͱ� ����. �������� �ٴڿ� ������ true, �ȴ����� false
        if (onGround || axisH != 0)
        {
            // ���� �� or �ӵ��� 0 �ƴ�
            // �ӵ� �����ϱ�
            rbody.velocity = new Vector2(speed * axisH, rbody.velocity.y);// axisH�� x�� 1*3=3��ŭ ������( (1,0)��(2,0)���� (2,0)�� (1,0)�� 2�� ����) == 3.0f�� ����ũ��, axisH�� ���� 
        }
        // rbody.velocity.y�� �긦 �ٽ� ����?? ���������� axisH,rbody.��� �� 0��. �������������鼭 �������� *3�� �������� ���������� y�� �߷��� ����.y�� 0,-9.8,0,-9.8�̷������� �ۿ��� >>
        // �׷��� 0�� ���ϰ� rbody.velocity.y�̰ɽἭ y���� ���⼭ �������� �ʰ� �ٸ������� �����ϴ� y���Ͱ��� �״�� ����϶�� ��
        // y�� ������ ����
        //rbody.velocity = new Vector2(axisH * 3.0f, 0);// y���� 0���� ������Ŵ �׷��� �������� ������ �߷����� >> x�����δ� ����*3���� �����̰� y�� ���� ����������
        if(onGround && goJump)
        {
            // ���� ������ ���� Ű ����
            // �����ϱ�
            Debug.Log("����!!");
            Vector2 jumpPw = new Vector2(0, jump); // ������ ���� ���� ����
            rbody.AddForce(jumpPw, ForceMode2D.Impulse); // ������ ���� ���� ��. Impulse�� ���������� ���� �ִ� ��
            goJump = false; // ���� ���� �÷��� ����
        }
    }
    // ����
    public void Jump()
    {
        goJump = true; // ���� �÷��� �ѱ�
        Debug.Log("���� ��ư ����!"); 

    }
}
