using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    Rigidbody2D rbody;
    float axisH = 0.0f;
    public float speed = 3.0f; // �ӵ��� public���� �����ؼ� �ν����Ϳ��� ������ �� �ֵ��� ��

    public float jump = 9.0f;// ������
    public LayerMask groundLayer; // ������ �� �ִ� ���̾� bool�� ����(on,off����) , 1byte
    bool goJump = false; // ���� ���� �÷���
    bool onGround = false; // �ٴڿ� �ִ��� üũ�ϴ� �÷���

    //�ִϸ��̼� ó��
    Animator animator; // �ִϸ��̼��� ���� ����
    public string stopAnime = "PlayerStop"; 
    public string moveAnime = "PlayerMove";
    public string jumpAnime = "PlayerJump";
    public string goalAnime = "PlayerGoal";
    public string deadAnime = "PlayerOver";
    string nowAnime = "";
    string oldAnime = ""; 

    public static string gameState = "Playing"; // ���� ���¸� ��Ÿ���� ����

    // �������� - ����
    int jumpCount = 0; // ���� ���� Ƚ��
    public int maxJumpCount = 2; // �ִ� ���� Ƚ��(2������)
    // �������� - ��

    // Start is called before the first frame update
    void Start()
    {
        rbody= this.GetComponent<Rigidbody2D>(); // ó������ �ѹ� �ҷ����ϱ� start�� ����
        // Animator ��������
        animator = GetComponent<Animator>(); 
        nowAnime = stopAnime; 
        oldAnime = stopAnime;

        gameState = "Playing"; // ������
    }

    // Update is called once per frame
    // update�� ������ ȣ��Ǵϱ� �� �ȿ� �÷��̾��� ���¸� ����üũ�ϴ� �ڵ带 �־� ����Ѵ�.
    // �׻� ���Ǻ��̴� ������ ������.
    void Update()
    {
        if(gameState != "Playing") return; // ���� ���°� Playing�� �ƴ� ��� �Լ� ����
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
        if (gameState != "Playing") return; // ���� ���°� Playing�� �ƴ� ��� �Լ� ����
        // ���� ���� 1
        //onGround = Physics2D.Linecast(transform.position,// ����ġ(�߹ٴڳ�)
        //                              transform.position - (transform.up * 0.1f), // 0.1f�� �ٴڰ��� �Ÿ� ���Ͱ��Ÿ��� ����� /transform.up�� ��������(1,0) > (1,0)*speed = (speed,0) �̱⶧���� ������(1,0)*0.1f = (0.1f,0) / transform.up�� y���� �������� �� ������
        //                              groundLayer); // �������� ��� �Ͱ� ����. �������� �ٴڿ� ������ true, �ȴ����� false

        // ���� ���� 2
        RaycastHit2D hit = Physics2D.Raycast(transform.position,Vector2.down,0.1f,groundLayer); // �������� ��� �Ͱ� ����. �������� �ٴڿ� ������ true, �ȴ����� false 
        onGround = hit.collider != null; // �������� �ٴڿ� ������ true, �ȴ����� false
        // linecast�� �ε����ٸ� �˼����� ��/?return�� ���  >> �ٵ� raycast�� hit�� �޾Ƽ� �׾ȿ� collider�� �˼��ְ� �̷������� ������ �� �˼�����(������ �������...���)


        if (onGround || axisH != 0)
        {
            // ���� �� or �ӵ��� 0 �ƴ� == ���߿� ������
            // �ӵ� �����ϱ�
            rbody.velocity = new Vector2(speed * axisH, rbody.velocity.y);// axisH�� x�� 1*3=3��ŭ ������( (1,0)��(2,0)���� (2,0)�� (1,0)�� 2�� ����) == 3.0f�� ����ũ��, axisH�� ���� 
        }
        // rbody.velocity.y�� �긦 �ٽ� ����?? ���������� axisH,rbody.��� �� 0��. �������������鼭 �������� *3�� �������� ���������� y�� �߷��� ����.y�� 0,-9.8,0,-9.8�̷������� �ۿ��� >>
        // �׷��� 0�� ���ϰ� rbody.velocity.y�̰ɽἭ y���� ���⼭ �������� �ʰ� �ٸ������� �����ϴ� y���Ͱ��� �״�� ����϶�� ��
        // y�� ������ ����
        //rbody.velocity = new Vector2(axisH * 3.0f, 0);// y���� 0���� ������Ŵ �׷��� �������� ������ �߷����� >> x�����δ� ����*3���� �����̰� y�� ���� ����������

        if(onGround)
        {
            jumpCount = 0; // ���� Ƚ�� �ʱ�ȭ
        }


        // ���� ����
        if (goJump)
        {
            // ���� ������ ���� Ű ����
            // �����ϱ�
            Vector2 jumpPw = new Vector2(0, jump); // ������ ���� ���� ����
            rbody.AddForce(jumpPw, ForceMode2D.Impulse); // ������ ���� ���� ��. Impulse�� ���������� ���� �ִ� ��
          
            goJump = false; // ���� ���� �÷��� ����
            
            
        }




        if (onGround)
        {
            // ���� ��
            if (axisH == 0)
            {
                nowAnime = stopAnime; // ���� �ִϸ��̼�
            }
            else
            {
                nowAnime = moveAnime; // �̵� �ִϸ��̼�
            }
        }
        else
        {
            // ����
            nowAnime = jumpAnime; // ���� �ִϸ��̼�
        }

        if (nowAnime != oldAnime)
        {
            oldAnime = nowAnime;
            animator.Play(nowAnime); // �ִϸ��̼� ���
        }

       
    }


    // ����
    public void Jump()
    {
        // goJump = true;

        // �������� - ����
        jumpCount++;  // ���� Ƚ�� ����
        if (jumpCount < maxJumpCount)
        {
            goJump = true;
        }
        // �������� - ��

    }

    // ���� ����
    // collider�� isTrigger �Ӽ��� ����� ��� �߻��ϴ� �̺�Ʈ �Լ�
    // isTrigger�� �浹�� �������ϰ� �̵��� ������ ����
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Goal")
        {
            Goal(); // ��
        }
        else if (collision.gameObject.tag == "Dead")
        {
            GameOver(); // ���ӿ���
        }
    }

    // collider�� isTrigger �Ӽ��� ������� �ʴ� ���(����Ʈ) �߻��ϴ� �̺�Ʈ �Լ�
    // �浹 ������ ���Ҿ� �̵��� ����(=��ó�� ����)
    //public void OnCollisionEnter2D(Collision2D collision)
    //{
    //    Debug.Log("OncollisionEnter2D �浹 �̺�Ʈ �߻�");
    //    if (collision.gameObject.tag == "Goal")
    //    {
    //        Goal(); // ��
    //    }
    //    else if (collision.gameObject.tag == "Dead")
    //    {
    //        GameOver(); // ���ӿ���
    //    }
    //}
    // ��
    public void Goal()
    {
        animator.Play(goalAnime); // �� �ִϸ��̼� ���
        gameState = "gameclear";
        GameStop(); // ���� ����
    }
    // ���ӿ���
    public void GameOver()
    {
       animator.Play(deadAnime); // ���ӿ��� �ִϸ��̼� ���

        gameState = "gameover";
        GameStop();
        // =============
        // ���ӿ��� ����
        // =============
        // �÷��̾��� �浹 ���� ��Ȱ��
        GetComponent<CapsuleCollider2D>().enabled = false; // �ѹ��� ���  
        // �÷��̾ ���� Ƣ�� ������ �ϴ� ����
        rbody.AddForce(new Vector2(0, 5), ForceMode2D.Impulse);
    }

    // ���� ����
    void GameStop()
    {
        // Ridbody2D ��������
        Rigidbody2D rbody= GetComponent<Rigidbody2D>(); // �÷��̾��� Ridbody2D ��������
        // �ӵ��� 0���� �Ͽ� ���� ����
        rbody.velocity = new Vector2(0, 0); // �ӵ��� 0���� �Ͽ� ���� ���� 
    }
}
