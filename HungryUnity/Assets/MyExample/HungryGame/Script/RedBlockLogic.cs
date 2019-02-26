using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RedBlockLogic : MonoBehaviour
{
    public bool IsRotate = false;
    public bool IsMoving = false;

    public float minRotateSpeed = 5;
    public float maxRotateSpeed = 15;

    public float minMovingSpeed = 5f;
    public float maxMovingSpeed = 10f;

    private float rotateSpeed = 0;
    private float moveSpeed = 0;
    private float moveDirection = 1;

    // Start is called before the first frame update
    void Start()
    {
        rotateSpeed = Random.Range(minRotateSpeed, maxRotateSpeed);
        moveSpeed = Random.Range(minMovingSpeed, maxMovingSpeed);
    }

    private float resetTime = 1f;
    private bool isNormalState = true;
    public void ResetArena()
    {
        resetTime = 1f;
        isNormalState = false;
        transform.localPosition = new Vector3(0, 100f, 0);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        resetTime -= Time.fixedDeltaTime;

        if (IsRotate)
        {
            if (resetTime < 0 && isNormalState == false)
            {
                isNormalState = true;
                transform.localPosition = new Vector3(0, 1.6f, 0);
            }

            if (isNormalState)
            {
                transform.Rotate(transform.up, Time.fixedDeltaTime * rotateSpeed);
            }
        }else if (IsMoving)
        {
            transform.Translate(Time.fixedDeltaTime * moveSpeed * transform.right * moveDirection, Space.World);

            if (transform.localPosition.x >= 50 || transform.localPosition.x <= -50)
            {
                moveDirection = moveDirection * -1;
            }
        }
        
    }
}
