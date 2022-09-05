using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Spine.Unity;

public class JoyStick : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    public RectTransform joyStickBG;                // 조이스틱 배경 이미지
    public RectTransform joyStickButton;            // 조이스틱 이미지

    public Transform targetCube;                    // 움직일 타겟

    Vector2 stickVector;                            // 스틱의 움직인 벡터 값
    Vector3 smoothVelocity;                         // SmoothDamp 3번째 인자값

    IEnumerator currCoroutine;                      // 현재 진행중인 코루틴 (OnJoyStickReset 정지하기 위해 사용)

    float smoothTime = 0.2f;                        // 스틱이 제자리로 돌아오는 시간 
    float smoothSpeed = 10.0f;                      // 스틱이 돌아오는 속도
    float inverseSmoothSpeed = 0f;                  // 곱연산을 하기위한 역수
    float bgRadius = 0f;                            // 조이스틱 배경 이미지의 반지름
    //float stickDistRatio = 0f;                      // 스틱이 움직인 거리 비율
    float moveSpeed = 5f;                           // 타겟의 이동 속도
    float rotSpeed = 10f;                           // 타겟의 회전 속도

    bool isReturn = false;                          // 스틱이 돌아오는 함수 실행중인지 판단하는 변수
    bool canMove = false;                           // 스틱을 움직이고 있는지 판단하는 변수

    SkeletonAnimation skeletonAnimation;
    Rigidbody2D rb;

    void Start()
    {
        skeletonAnimation = targetCube.GetComponent<SkeletonAnimation>();
        skeletonAnimation.state.SetAnimation(0, "Idle_1", true);

        rb = targetCube.GetComponent<Rigidbody2D>();

        bgRadius = joyStickBG.rect.width * 0.5f;
        inverseSmoothSpeed = 1 / smoothSpeed;
    }


    void Update()
    {
        MoveAndRotate();
    }

    public float speed;
    public float line;

    // Target의 이동 및 회전
    void MoveAndRotate()
    {
        if (!canMove) return;

        // 이동
        Vector2 normalVec = stickVector.normalized;
        //rb.velocity = (new Vector3(normalVec.x, normalVec.y, 0) * speed * Time.deltaTime);
        RaycastHit2D hit = Physics2D.Linecast(targetCube.position, targetCube.position + new Vector3(normalVec.x, normalVec.y, 0) * line, 1 << 3);
        dir = normalVec;
        if (hit)
        {
            return;
        }

        targetCube.position += new Vector3(normalVec.x, normalVec.y, 0) * speed * Time.deltaTime;

        if (normalVec.x >= 0)
            skeletonAnimation.skeleton.ScaleX = 1;
        else
            skeletonAnimation.skeleton.ScaleX = -1;

        // 회전
        // Vector3 newRot = Vector3.up * Mathf.Atan2(normalVec.x, normalVec.y) * Mathf.Rad2Deg;
        // targetCube.rotation = Quaternion.Lerp(targetCube.rotation, Quaternion.Euler(newRot), rotSpeed * Time.deltaTime);
    }

    Vector3 dir;
    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(targetCube.position, targetCube.position + dir * line);
    }

    // 마우스 클릭 위치에 따른 조이스틱의 이동
    void OnJoyStickMove(Vector2 pointerPos)
    {
        if (isReturn)
        {
            StopCoroutine(currCoroutine);
            isReturn = false;
        }

        // 클릭 위치가 중앙으로부터 얼마나 떨어져있는지 계산 후
        // 배경 이미지의 크기를 넘어가지 않도록 제한
        stickVector = new Vector2(pointerPos.x - joyStickBG.position.x, pointerPos.y - joyStickBG.position.y);
        stickVector = Vector2.ClampMagnitude(stickVector, bgRadius);

        joyStickButton.localPosition = stickVector;

        // 조이스틱의 위치와 배경 이미지의 반지름 비율
        //stickDistRatio = (joyStickBG.position - joyStickButton.position).sqrMagnitude / (bgRadius * bgRadius);
    }

    // 조이스틱이 초기 위치로 돌아옴
    IEnumerator OnJoyStickReset()
    {
        isReturn = true;

        float time = Time.time;

        while (Time.time < time + smoothTime)
        {
            joyStickButton.position = Vector3.SmoothDamp(joyStickButton.position, joyStickBG.position, ref smoothVelocity, inverseSmoothSpeed);
            yield return null;
        }

        isReturn = false;
        joyStickButton.position = joyStickBG.position;
    }

    // 마우스 클릭할 때 이벤트 발생
    public void OnPointerDown(PointerEventData eventData)
    {
        canMove = true;
        OnJoyStickMove(eventData.position);

        skeletonAnimation.state.SetAnimation(0, "Run_NoHand", true);
    }

    // 마우스 누르고 있을 때 이벤트 발생
    public void OnDrag(PointerEventData eventData)
    {
        OnJoyStickMove(eventData.position);
    }

    // 마우스 클릭 끝날 때 이벤트 발생
    public void OnPointerUp(PointerEventData eventData)
    {
        canMove = false;

        // 손 놨을 때 원래 자리로 이동
        currCoroutine = OnJoyStickReset();
        StartCoroutine(currCoroutine);

        rb.velocity = Vector3.zero;
        skeletonAnimation.state.SetAnimation(0, "Idle_1", true);
    }
}

