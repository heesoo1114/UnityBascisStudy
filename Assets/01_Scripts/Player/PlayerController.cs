using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    Rigidbody _rigid;
    Transform _modelTr;

    [Header("Movement")]
    public float sideSpeed;
    // [SerializeField] private float frontSpeed;

    [Header("Dash")]
    public float dashSpeed;
    public float dashDuration = 0.5f;
    private bool isDashing = false;

    [Header("Roatation")]
    public float rollAmount;
    public float lerpAmount;
    Vector3 rotateValue;

    private void Awake()
    {
        _rigid = GetComponent<Rigidbody>();
        _modelTr = transform.Find("Model").GetComponent<Transform>();
    }

    private void Update()
    {
        // 입력 부분 나중에 모바일 버전으로 수정하기
        float z = Input.GetAxisRaw("Horizontal");
        if (Input.GetKeyDown(KeyCode.Space))
        {
            DashMovement(z);
        }

        RotationPlane(z);
        HorizontalMovement(z);
    }

    private void HorizontalMovement(float x)
    {
        Vector3 movement = new Vector3(x, 0f, 0f);
        _rigid.velocity = movement * sideSpeed;
    }

    private void RotationPlane(float z)
    {
        Vector3 lerpVector = new Vector3(0, 0, -z * rollAmount);
        rotateValue = Vector3.Lerp(rotateValue, lerpVector, lerpAmount * Time.deltaTime);

        _modelTr.rotation *= Quaternion.Euler(rotateValue * Time.fixedDeltaTime);

        // _rigid.MoveRotation(_rigid.rotation * Quaternion.Euler(rotateValue * Time.fixedDeltaTime));
    }

    private void DashMovement(float z)
    {
        Vector3 dashDir = (z == 1) ? Vector3.right : Vector3.left;

        StartCoroutine(Dash(dashDir));

        // _rigid.AddForce(dashDir * dashSpeed, ForceMode.Impulse); 
        // _rigid.velocity = dashDir * dashSpeed;
    }

    private IEnumerator Dash(Vector3 dashDir)
    {
        isDashing = true;
        Vector3 dashDirection = dashDir;
        float elapsedTime = 0f;

        while (elapsedTime < dashDuration)
        {
            float dashSpeedCurrent = Mathf.Lerp(dashSpeed, 0f, elapsedTime / dashDuration);

            _rigid.velocity = dashDirection * dashSpeedCurrent;

            elapsedTime += Time.deltaTime;
            yield return null;
        }

        _rigid.velocity = Vector3.zero;
        isDashing = false;
    }

    private void StopImmediatelly()
    {
        // frontSpeed = 0;
        // verticalSpeed = 0;
        sideSpeed = 0;
        dashSpeed = 0;
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Collision Happen");
    }

    #region Accel,Decel
    // private float AccelSpeed()
    // {
    //     moveSpeed += acceleration * Time.deltaTime;
    // 
    //     return Mathf.Clamp(moveSpeed, 0, maxSpeed);
    // }
    // 
    // private float DecelSpeed()
    // {
    //     moveSpeed -= deAcceleration * Time.deltaTime;
    // 
    //     return Mathf.Clamp(moveSpeed, 0, maxSpeed);
    // }
    #endregion
}
