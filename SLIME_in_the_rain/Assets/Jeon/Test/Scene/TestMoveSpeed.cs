using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestMoveSpeed : MonoBehaviour
{
  

    private void FixedUpdate()
    {
        Move();
    }
    enum AnimState { idle, move, dash, damaged, die }     // 애니메이션의 상태
    AnimState animState = AnimState.idle;
    private Vector3 direction;                  // 이동 방향
    private Animator anim;
    public float time;

    public float moveSpeed;
    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    void Move()
    {

        float dirX = Input.GetAxis("Horizontal");
        float dirZ = Input.GetAxis("Vertical");

        if (dirX != 0 || dirZ != 0)
        {
            animState = AnimState.move;

            direction = new Vector3(dirX, 0, dirZ);

            if (direction != Vector3.zero)
            {
                float angle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg;

                transform.rotation = Quaternion.Euler(0, angle, 0);         // 회전
            }
            time += Time.deltaTime;
            transform.position += direction *2 * moveSpeed * Time.deltaTime;   // 이동
        }
        else
        {
            animState = AnimState.idle;
        }

        PlayAnim(animState);
    }
    void PlayAnim(AnimState state)
    {
        animState = state;

        anim.SetInteger("animation", (int)animState);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Trap"))
        {
            Debug.Log(time);
           
        }
    }

    private Vector3 oldPosition;
    private Vector3 currentPosition;
    private double velocity;

    void Start()
    {
        oldPosition = transform.position;
    }

    void Update()
    {
        currentPosition = transform.position;
        var dis = (currentPosition - oldPosition);
        var distance = Mathf.Sqrt(Mathf.Pow(dis.x, 2) + Mathf.Pow(dis.y, 2) + Mathf.Pow(dis.z, 2));
        velocity = distance / Time.deltaTime;
        oldPosition = currentPosition;

       // Debug.Log("속도 :" + velocity);
    }



}
