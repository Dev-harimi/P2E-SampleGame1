using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Spine.Unity;

public class JoyStick : MonoBehaviour, IPointerDownHandler, IDragHandler, IPointerUpHandler
{
    public RectTransform joyStickBG;                // ���̽�ƽ ��� �̹���
    public RectTransform joyStickButton;            // ���̽�ƽ �̹���

    public Transform targetCube;                    // ������ Ÿ��

    Vector2 stickVector;                            // ��ƽ�� ������ ���� ��
    Vector3 smoothVelocity;                         // SmoothDamp 3��° ���ڰ�

    IEnumerator currCoroutine;                      // ���� �������� �ڷ�ƾ (OnJoyStickReset �����ϱ� ���� ���)

    float smoothTime = 0.2f;                        // ��ƽ�� ���ڸ��� ���ƿ��� �ð� 
    float smoothSpeed = 10.0f;                      // ��ƽ�� ���ƿ��� �ӵ�
    float inverseSmoothSpeed = 0f;                  // �������� �ϱ����� ����
    float bgRadius = 0f;                            // ���̽�ƽ ��� �̹����� ������
    //float stickDistRatio = 0f;                      // ��ƽ�� ������ �Ÿ� ����
    float moveSpeed = 5f;                           // Ÿ���� �̵� �ӵ�
    float rotSpeed = 10f;                           // Ÿ���� ȸ�� �ӵ�

    bool isReturn = false;                          // ��ƽ�� ���ƿ��� �Լ� ���������� �Ǵ��ϴ� ����
    bool canMove = false;                           // ��ƽ�� �����̰� �ִ��� �Ǵ��ϴ� ����

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

    // Target�� �̵� �� ȸ��
    void MoveAndRotate()
    {
        if (!canMove) return;

        // �̵�
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

        // ȸ��
        // Vector3 newRot = Vector3.up * Mathf.Atan2(normalVec.x, normalVec.y) * Mathf.Rad2Deg;
        // targetCube.rotation = Quaternion.Lerp(targetCube.rotation, Quaternion.Euler(newRot), rotSpeed * Time.deltaTime);
    }

    Vector3 dir;
    private void OnDrawGizmos()
    {
        Gizmos.DrawLine(targetCube.position, targetCube.position + dir * line);
    }

    // ���콺 Ŭ�� ��ġ�� ���� ���̽�ƽ�� �̵�
    void OnJoyStickMove(Vector2 pointerPos)
    {
        if (isReturn)
        {
            StopCoroutine(currCoroutine);
            isReturn = false;
        }

        // Ŭ�� ��ġ�� �߾����κ��� �󸶳� �������ִ��� ��� ��
        // ��� �̹����� ũ�⸦ �Ѿ�� �ʵ��� ����
        stickVector = new Vector2(pointerPos.x - joyStickBG.position.x, pointerPos.y - joyStickBG.position.y);
        stickVector = Vector2.ClampMagnitude(stickVector, bgRadius);

        joyStickButton.localPosition = stickVector;

        // ���̽�ƽ�� ��ġ�� ��� �̹����� ������ ����
        //stickDistRatio = (joyStickBG.position - joyStickButton.position).sqrMagnitude / (bgRadius * bgRadius);
    }

    // ���̽�ƽ�� �ʱ� ��ġ�� ���ƿ�
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

    // ���콺 Ŭ���� �� �̺�Ʈ �߻�
    public void OnPointerDown(PointerEventData eventData)
    {
        canMove = true;
        OnJoyStickMove(eventData.position);

        skeletonAnimation.state.SetAnimation(0, "Run_NoHand", true);
    }

    // ���콺 ������ ���� �� �̺�Ʈ �߻�
    public void OnDrag(PointerEventData eventData)
    {
        OnJoyStickMove(eventData.position);
    }

    // ���콺 Ŭ�� ���� �� �̺�Ʈ �߻�
    public void OnPointerUp(PointerEventData eventData)
    {
        canMove = false;

        // �� ���� �� ���� �ڸ��� �̵�
        currCoroutine = OnJoyStickReset();
        StartCoroutine(currCoroutine);

        rb.velocity = Vector3.zero;
        skeletonAnimation.state.SetAnimation(0, "Idle_1", true);
    }
}

