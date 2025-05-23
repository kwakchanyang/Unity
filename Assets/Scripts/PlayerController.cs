using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    Rigidbody2D rbody;
    float axisH = 0.0f;
    public float speed = 3.0f; // 속도는 public으로 설정해서 인스펙터에서 조정할 수 있도록 함

    public float jump = 9.0f;// 점프력
    public LayerMask groundLayer; // 착지할 수 있는 레이어
    bool goJump = false; // 점프 개시 플래그
    bool onGround = false; // 바닥에 있는지 체크하는 플래그

    // Start is called before the first frame update
    void Start()
    {
        rbody= this.GetComponent<Rigidbody2D>(); // 처음에만 한번 불러오니까 start에 적음
    }

    // Update is called once per frame
    // update는 어차피 호출되니까 이 안에 플레이어의 상태를 지속체크하는 코드를 넣어 사용한다.
    // 항상 조건붙이는 습관을 들이자.
    void Update()
    {
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
        // 착지 판정
        onGround = Physics2D.Linecast(transform.position,
                                      transform.position - (transform.up * 0.1f), // 0.1f는 바닥과의 거리
                                      groundLayer); // 레이저를 쏘는 것과 같음. 레이저가 바닥에 닿으면 true, 안닿으면 false
        if (onGround || axisH != 0)
        {
            // 지면 위 or 속도가 0 아님
            // 속도 갱신하기
            rbody.velocity = new Vector2(speed * axisH, rbody.velocity.y);// axisH는 x임 1*3=3만큼 빠르다( (1,0)과(2,0)에서 (2,0)은 (1,0)의 2배 빠름) == 3.0f는 힘의크기, axisH는 방향 
        }
        // rbody.velocity.y왜 얘를 다시 넣지?? 서있을때는 axisH,rbody.얘네 다 0임. 오른쪽을누르면서 떨어지는 *3의 방향으로 떨어지지만 y는 중력을 받음.y는 0,-9.8,0,-9.8이런식으로 작용함 >>
        // 그래서 0을 안하고 rbody.velocity.y이걸써서 y값은 여기서 조정하지 않고 다른곳에서 적용하는 y벡터값을 그대로 사용하라는 뜻
        // y는 기존값 유지
        //rbody.velocity = new Vector2(axisH * 3.0f, 0);// y축은 0으로 고정시킴 그래도 떨어지는 이유는 중력떄문임 >> x축으로는 방향*3으로 움직이고 y는 절대 움직이지마
        if(onGround && goJump)
        {
            // 지면 위에서 점프 키 눌림
            // 점프하기
            Debug.Log("점프!!");
            Vector2 jumpPw = new Vector2(0, jump); // 점프를 위한 벡터 생성
            rbody.AddForce(jumpPw, ForceMode2D.Impulse); // 점프를 위한 힘을 줌. Impulse는 순간적으로 힘을 주는 것
            goJump = false; // 점프 개시 플래그 끄기
        }
    }
    // 점프
    public void Jump()
    {
        goJump = true; // 점프 플래그 켜기
        Debug.Log("점프 버튼 눌림!"); 

    }
}
