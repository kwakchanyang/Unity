using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    Rigidbody2D rbody;
    float axisH = 0.0f;
    public float speed = 3.0f; // 속도는 public으로 설정해서 인스펙터에서 조정할 수 있도록 함

    public float jump = 9.0f;// 점프력
    public LayerMask groundLayer; // 착지할 수 있는 레이어 bool값 관리(on,off상태) , 1byte
    bool goJump = false; // 점프 개시 플래그
    bool onGround = false; // 바닥에 있는지 체크하는 플래그

    //애니메이션 처리
    Animator animator; // 애니메이션을 위한 변수
    public string stopAnime = "PlayerStop"; 
    public string moveAnime = "PlayerMove";
    public string jumpAnime = "PlayerJump";
    public string goalAnime = "PlayerGoal";
    public string deadAnime = "PlayerOver";
    string nowAnime = "";
    string oldAnime = ""; 

    public static string gameState = "Playing"; // 게임 상태를 나타내는 변수

    // 이중점프 - 시작
    int jumpCount = 0; // 현재 점프 횟수
    public int maxJumpCount = 2; // 최대 점프 횟수(2단점프)
    // 이중점프 - 끝

    // Start is called before the first frame update
    void Start()
    {
        rbody= this.GetComponent<Rigidbody2D>(); // 처음에만 한번 불러오니까 start에 적음
        // Animator 가져오기
        animator = GetComponent<Animator>(); 
        nowAnime = stopAnime; 
        oldAnime = stopAnime;

        gameState = "Playing"; // 게임중
    }

    // Update is called once per frame
    // update는 어차피 호출되니까 이 안에 플레이어의 상태를 지속체크하는 코드를 넣어 사용한다.
    // 항상 조건붙이는 습관을 들이자.
    void Update()
    {
        if(gameState != "Playing") return; // 게임 상태가 Playing이 아닐 경우 함수 종료
        axisH = Input.GetAxisRaw("Horizontal");// 키가 지속적으로 눌렸는지를 계속 확인 / 아무것도안누름면0, 왼쪽키면-1, 오른쪽키면1
        // 방향 조절
        if ( axisH > 0.0f) // 0일때 이걸 실행해 xxx ex)2m범위에 들어오면 공격해 > 2보다 크면, 2보다작으면 이렇게 함(정확한 숫자가 딱 나오기 힘듦) 등호는 쓰지 않음!!!
        {
            // 오른쪽 이동
            Debug.Log("오른쪽 이동"); // 디버그로 확인
            transform.localScale = new Vector2(1, 1); // 원래 this가 붙어있음 . 플레이어 객체의 transform은 localScale을 가져옴.(부모의 값에 상대적임) 
        }
        else if ( axisH < 0.0f)
        {
            // 왼쪽 이동
            Debug.Log("왼쪽 이동"); // 디버그로 확인
            transform.localScale = new Vector2(-1, 1);  // 좌우반전
        }

        // 캐릭터 점프
        if( Input.GetButtonDown("Jump"))
        {
            Jump();// 점프
        }

    }

    // 
    void FixedUpdate()
    {
        if (gameState != "Playing") return; // 게임 상태가 Playing이 아닐 경우 함수 종료
        // 착지 판정 1
        //onGround = Physics2D.Linecast(transform.position,// 내위치(발바닥끝)
        //                              transform.position - (transform.up * 0.1f), // 0.1f는 바닥과의 거리 벡터간거리는 빼면됨 /transform.up는 단위벡터(1,0) > (1,0)*speed = (speed,0) 이기때문에 지금은(1,0)*0.1f = (0.1f,0) / transform.up는 y축을 기준으로 한 벡터임
        //                              groundLayer); // 레이저를 쏘는 것과 같음. 레이저가 바닥에 닿으면 true, 안닿으면 false

        // 착지 판정 2
        RaycastHit2D hit = Physics2D.Raycast(transform.position,Vector2.down,0.1f,groundLayer); // 레이저를 쏘는 것과 같음. 레이저가 바닥에 닿으면 true, 안닿으면 false 
        onGround = hit.collider != null; // 레이저가 바닥에 닿으면 true, 안닿으면 false
        // linecast는 부딪혔다만 알수있음 왜/?return이 없어서  >> 근데 raycast는 hit를 받아서 그안에 collider을 알수있고 이런식으로 정보를 더 알수있음(땅인지 블록인지...등등)


        if (onGround || axisH != 0)
        {
            // 지면 위 or 속도가 0 아님 == 공중에 떠있음
            // 속도 갱신하기
            rbody.velocity = new Vector2(speed * axisH, rbody.velocity.y);// axisH는 x임 1*3=3만큼 빠르다( (1,0)과(2,0)에서 (2,0)은 (1,0)의 2배 빠름) == 3.0f는 힘의크기, axisH는 방향 
        }
        // rbody.velocity.y왜 얘를 다시 넣지?? 서있을때는 axisH,rbody.얘네 다 0임. 오른쪽을누르면서 떨어지는 *3의 방향으로 떨어지지만 y는 중력을 받음.y는 0,-9.8,0,-9.8이런식으로 작용함 >>
        // 그래서 0을 안하고 rbody.velocity.y이걸써서 y값은 여기서 조정하지 않고 다른곳에서 적용하는 y벡터값을 그대로 사용하라는 뜻
        // y는 기존값 유지
        //rbody.velocity = new Vector2(axisH * 3.0f, 0);// y축은 0으로 고정시킴 그래도 떨어지는 이유는 중력떄문임 >> x축으로는 방향*3으로 움직이고 y는 절대 움직이지마

        if(onGround)
        {
            jumpCount = 0; // 점프 횟수 초기화
        }


        // 이중 점프
        if (goJump)
        {
            // 지면 위에서 점프 키 눌림
            // 점프하기
            Vector2 jumpPw = new Vector2(0, jump); // 점프를 위한 벡터 생성
            rbody.AddForce(jumpPw, ForceMode2D.Impulse); // 점프를 위한 힘을 줌. Impulse는 순간적으로 힘을 주는 것
          
            goJump = false; // 점프 개시 플래그 끄기
            
            
        }




        if (onGround)
        {
            // 지면 위
            if (axisH == 0)
            {
                nowAnime = stopAnime; // 정지 애니메이션
            }
            else
            {
                nowAnime = moveAnime; // 이동 애니메이션
            }
        }
        else
        {
            // 공중
            nowAnime = jumpAnime; // 점프 애니메이션
        }

        if (nowAnime != oldAnime)
        {
            oldAnime = nowAnime;
            animator.Play(nowAnime); // 애니메이션 재생
        }

       
    }


    // 점프
    public void Jump()
    {
        // goJump = true;

        // 이중점프 - 시작
        jumpCount++;  // 점프 횟수 증가
        if (jumpCount < maxJumpCount)
        {
            goJump = true;
        }
        // 이중점프 - 끝

    }

    // 접촉 시작
    // collider의 isTrigger 속성을 사용할 경우 발생하는 이벤트 함수
    // isTrigger는 충돌을 감지만하고 이동을 막지는 않음
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.tag == "Goal")
        {
            Goal(); // 골
        }
        else if (collision.gameObject.tag == "Dead")
        {
            GameOver(); // 게임오버
        }
    }

    // collider의 isTrigger 속성을 사용하지 않는 경우(디폴트) 발생하는 이벤트 함수
    // 충돌 감지와 더불어 이동을 막음(=벽처럼 동작)
    //public void OnCollisionEnter2D(Collision2D collision)
    //{
    //    Debug.Log("OncollisionEnter2D 충돌 이벤트 발생");
    //    if (collision.gameObject.tag == "Goal")
    //    {
    //        Goal(); // 골
    //    }
    //    else if (collision.gameObject.tag == "Dead")
    //    {
    //        GameOver(); // 게임오버
    //    }
    //}
    // 골
    public void Goal()
    {
        animator.Play(goalAnime); // 골 애니메이션 재생
        gameState = "gameclear";
        GameStop(); // 게임 중지
    }
    // 게임오버
    public void GameOver()
    {
       animator.Play(deadAnime); // 게임오버 애니메이션 재생

        gameState = "gameover";
        GameStop();
        // =============
        // 게임오버 연출
        // =============
        // 플레이어의 충돌 판정 비활성
        GetComponent<CapsuleCollider2D>().enabled = false; // 한번만 띄움  
        // 플레이어를 위로 튀어 오르게 하는 연출
        rbody.AddForce(new Vector2(0, 5), ForceMode2D.Impulse);
    }

    // 게임 중지
    void GameStop()
    {
        // Ridbody2D 가져오기
        Rigidbody2D rbody= GetComponent<Rigidbody2D>(); // 플레이어의 Ridbody2D 가져오기
        // 속도를 0으로 하여 강제 정지
        rbody.velocity = new Vector2(0, 0); // 속도를 0으로 하여 강제 정지 
    }
}
