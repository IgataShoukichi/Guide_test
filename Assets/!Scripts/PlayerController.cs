using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] Rigidbody rb;
    [Header("移動速度")]
    public float speed = 5f;
    [System.NonSerialized] public Vector3 input;

    [Header("メインカメラのTransform")]
    [SerializeField] Transform cameraTran;

    [Header("客を誘導可能な時に表示するUI")]
    [SerializeField] CanvasGroup guideCanvas;

    [Header("Transformも取得するとGetComponentが発生するのでキャッシュしておく")]
    public Transform myTran;

    private bool isGuide = false, isContact = false, isTable_Player = false;
    private GameObject customer;

    // Update is called once per frame
    void Update()
    {
        input = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));

        var horizontalRotation = Quaternion.AngleAxis(cameraTran.eulerAngles.y, Vector3.up);

        var velocity = horizontalRotation * input;

        if (velocity.sqrMagnitude > 0.01f)
        {
            myTran.rotation = Quaternion.LookRotation(velocity);
        }

        if (velocity.sqrMagnitude > 1)
        {
            velocity = velocity.normalized;
        }

        rb.velocity = new Vector3(velocity.x * speed, rb.velocity.y, velocity.z * speed);

        if (Input.GetKeyDown(KeyCode.E))
        {
            if (isContact && !isGuide)
            {
                var customerControllers = customer.GetComponentsInChildren<CustomerController>();
                foreach (var controller in customerControllers)
                {
                    controller.DoFollow(this);
                }
                isGuide = true;
            }
            else if (isGuide && isTable_Player && customer != null)
            {
                var customerControllers = customer.GetComponentsInChildren<CustomerController>();
                foreach (var controller in customerControllers)
                {
                    controller.StopFollow();
                }
                isGuide = false;
            }
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Customer"))
        {
            customer = other.gameObject;
            if (!isGuide)
            {
                isContact = true;

                guideCanvas.alpha = 1;
            }
        }
        if (other.CompareTag("Table") && isGuide)
        {
            isTable_Player = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Customer"))
        {
            isContact = false;
            customer = null;

            guideCanvas.alpha = 0;
        }
        if (other.CompareTag("Table") && isGuide)
        {
            isTable_Player = false;
        }

    }
}
