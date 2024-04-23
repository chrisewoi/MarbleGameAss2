using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Player : MonoBehaviour
{
    public Vector3[] velocities = new Vector3[5];
    Vector3[] velocityRefs = new Vector3[5];
    public float[] velocitySmoothTimes = new float[5];

    public enum VelocityType
    {
        Move, Jump, Gravity, Powerup1, Powerup2
    }
    Rigidbody rb;
    public bool grounded;

    // Applied each frame
    public Vector3 gravity;
    public Vector3 moveInput;
    Camera mainCamera;
    Vector3 cameraForward, cameraRight;
    public float moveSpeedMax;
    bool jumpInput;
    //set jump velocity increment
    public float jumpHeight;
    Vector3 spawnPosition;
    Vector3 startPosition;
    public static float moveSpeedMultiplier = 1;
    public static float moveSpeedBoostTime;
    public static float jumpHeightBoostTime;

    public float timer;
    public float checkpointedTime;
    public TextMeshProUGUI timerText;
    public TextMeshProUGUI highScoreText;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        mainCamera = Camera.main;
        InputManager.Subscribe_MoveInput(MoveInputHandler);
        InputManager.Subscribe_JumpInput(JumpInputHandler);
        startPosition = transform.position;
        spawnPosition = transform.position;
    }

    void MoveInputHandler(Vector2 moveInput)
    {
        this.moveInput = new Vector3(moveInput.x, 0f, moveInput.y);
    }

    void JumpInputHandler(float jumpInputHeldTime)
    {
        jumpInput = jumpInputHeldTime > 0;
    }


    void Update()
    {
        cameraForward = mainCamera.transform.forward;
        cameraRight = mainCamera.transform.right;
        cameraForward.y = 0f;
        cameraRight.y = 0f;
        cameraForward.Normalize();
        cameraRight.Normalize();


        if(moveSpeedBoostTime > 0)
        {
            moveSpeedBoostTime -= Time.deltaTime;
            if(moveSpeedBoostTime <= 0)
            {
                moveSpeedMultiplier = 1;
                moveSpeedBoostTime = 0;
            }
        }
        if(jumpHeightBoostTime > 0)
        {
            jumpHeightBoostTime -= Time.deltaTime;
            if(jumpHeightBoostTime <= 0)
            {
                jumpHeightBoostTime = 0;
            }
        }

        timer += Time.deltaTime;
        timerText.text = $"time = {timer}";

        float highScore = PlayerPrefs.GetFloat("HighScore");
        if (highScore == 0)
        {
            highScoreText.text = "No high score yet";
        }
        else
        {
            highScoreText.text = $"time to beat = {highScore}";
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        int moveIndex = (int)VelocityType.Move;
        velocities[moveIndex] += (cameraForward * moveInput.z + cameraRight * moveInput.x)*moveSpeedMultiplier;
        if (moveInput.magnitude < 0.2f)
        {
            velocities[moveIndex] = Vector3.SmoothDamp(velocities[moveIndex], Vector3.zero, ref velocityRefs[moveIndex], velocitySmoothTimes[moveIndex]);
        }
        velocities[moveIndex] = Vector3.ClampMagnitude(velocities[moveIndex], moveSpeedMax*moveSpeedMultiplier);

        int jumpIndex = (int)VelocityType.Jump;
        if(jumpInput && grounded)
        {
            velocities[jumpIndex] += new Vector3(0f, jumpHeight, 0f);
            if(jumpHeightBoostTime <= 0)
            {
                velocities[jumpIndex] = Vector3.ClampMagnitude(velocities[jumpIndex], jumpHeight);
            }
        }
        else
        {
            velocities[jumpIndex] = Vector3.SmoothDamp(velocities[jumpIndex], Vector3.zero, ref velocityRefs[jumpIndex], velocitySmoothTimes[jumpIndex]);
        }

        if (!grounded)
        {
            velocities[(int)VelocityType.Gravity] += gravity;
        } else
        {
            velocities[(int)VelocityType.Gravity] = Vector3.zero;
        }

        Vector3 finalVelocity = Vector3.zero;

        foreach (var v in velocities)
        {
            finalVelocity += v;
        }
        rb.velocity = finalVelocity;
    }

    void OnCollisionEnter(Collision other)
    {
        if (other.collider.CompareTag("Ground"))
        {
            grounded = true;
        }
        if(other.collider.CompareTag("KillArea"))
        {
            transform.position = spawnPosition;
            ResetVelocities();
            timer = checkpointedTime;
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Checkpoint"))
        {
            // Sets new spawn point to the middle of the checkpoint area
            spawnPosition = other.transform.position + new Vector3(0, 5, 0);
            checkpointedTime = timer;
        }
        if(other.CompareTag("End"))
        {
            float previousTime = PlayerPrefs.GetFloat("HighScore");
            if(timer < previousTime || previousTime <= 0)
            {
                PlayerPrefs.SetFloat("HighScore", timer);

            }

            spawnPosition = startPosition;
            checkpointedTime = 0;
        }
    }
    void OnCollisionStay(Collision other)
    {
        if (other.collider.CompareTag("KillArea"))
        {
            transform.position = spawnPosition;
            ResetVelocities();
            timer = checkpointedTime;
        }
    }
    void ResetVelocities()
    {
        //Set all velocities to zero
        for (int i = 0; i < velocities.Length; i++)
        {
            velocities[i] = Vector3.zero;
        }
    }

    void OnCollisionExit(Collision other)
    {
        if (other.collider.CompareTag("Ground"))
        {
            grounded = false;
        }
    }

    public static void SpeedBoost()
    {
        moveSpeedMultiplier = 3;
        moveSpeedBoostTime = 6;
    }

    public static void JumpBoost()
    {
        jumpHeightBoostTime = 1f;
    }
}