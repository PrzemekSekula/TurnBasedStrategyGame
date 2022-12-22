using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class CameraController : MonoBehaviour
{
    private const float MIN_FOLLOW_Y_OFFSET = 2f;
    private const float MAX_FOLLOW_Y_OFFSET = 12f;
    [SerializeField] private CinemachineVirtualCamera cinemachineVirtualCamera;


    private Vector3 targetFollowOffset;
    private CinemachineTransposer cinemachineTransposer;
    // Start is called before the first frame update
    void Start()
    {   
        cinemachineTransposer = cinemachineVirtualCamera.GetCinemachineComponent<CinemachineTransposer>();
        targetFollowOffset = cinemachineTransposer.m_FollowOffset;
    }

    // Update is called once per frame
    void Update()
    {
        HandleMovement();
        HandleRotation();
        HandleZoom();
    }

    private void HandleMovement()
    {
        Vector3 inputMoveDir = new Vector3(0, 0, 0);
        if (Input.GetKey(KeyCode.W))
        {
            inputMoveDir += Vector3.forward;
        }
        if (Input.GetKey(KeyCode.S))
        {
            inputMoveDir += Vector3.back;
        }
        if (Input.GetKey(KeyCode.A))
        {
            inputMoveDir += Vector3.left;
        }
        if (Input.GetKey(KeyCode.D))
        {
            inputMoveDir += Vector3.right;
        }

        float moveSpeed = 10f;

        Vector3 moveVector = transform.forward * inputMoveDir.z + transform.right * inputMoveDir.x;        
        transform.position += moveVector * moveSpeed * Time.deltaTime;    

    }

    private void HandleRotation()
    {
        Vector3 rotationVector = new Vector3(0, 0, 0);
        if (Input.GetKey(KeyCode.Q))
        {
            rotationVector += Vector3.up;
        }        
        if (Input.GetKey(KeyCode.E))
        {
            rotationVector += Vector3.down;
        }

        float rotationSpeed = 100f;
        transform.eulerAngles += rotationVector * rotationSpeed * Time.deltaTime;
    }

    private void HandleZoom()
    {
        float zoomAmount = 1f;        
        if (Input.mouseScrollDelta.y > 0)
        {
            targetFollowOffset.y -= zoomAmount;
        }
        if (Input.mouseScrollDelta.y < 0)
        {
            targetFollowOffset.y += zoomAmount;
        }

        targetFollowOffset.y = Mathf.Clamp(targetFollowOffset.y, MIN_FOLLOW_Y_OFFSET, MAX_FOLLOW_Y_OFFSET);

        float followSpeed = 5f;
        cinemachineTransposer.m_FollowOffset =
         Vector3.Lerp(cinemachineTransposer.m_FollowOffset, targetFollowOffset, followSpeed * Time.deltaTime);

    }
}
