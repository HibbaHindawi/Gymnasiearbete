// script written by Johannes and inspired by Dave/GameDevelopment Youtube

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
public class PlayerMovement : MonoBehaviour
{
    //Ground
    public float groundSpeed = 11f;
    public float runSpeed = 7f;
    public float grAccel = 45f;

    //Air
    public float airSpeed = 6f;
    public float airAccel = 45f;

    //Jump
    public float jumpUpSpeed = 9.2f;
    public float dashSpeed = 13f;

    //Wall
    public float wallSpeed = 10f;
    public float wallClimbSpeed = 4f;
    public float wallAccel = 20f;
    public float wallRunTime = 3f;
    public float wallStickiness = 20f;
    public float wallStickDistance = 1f;
    public float wallFloorBarrier = 40f;
    public float wallBanTime = 3f;
    Vector3 bannedGroundNormal;

    //Cooldowns
    public bool canJump = true;
    public bool canDJump = true;
    public float wallBan = 0f;
    public float wrTimer = 0f;
    public float wallStickTimer = 0f;

    //States
    public bool running;
    public bool jump;
    public bool crouched;
    public bool grounded;
    public bool Wallruning;
    Collider ground;

    Vector3 groundNormal = Vector3.up;

    CapsuleCollider col;

    enum Mode
    {
        Walking,
        Flying,
        Wallruning
    }
    Mode mode = Mode.Flying;

    CameraController camCon;
    Rigidbody rb;
    Vector3 dir = Vector3.zero;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        camCon = GetComponentInChildren<CameraController>();
        col = GetComponent<CapsuleCollider>();
    }

    public void OnGUI()
    {
        GUILayout.Label("Speed: " + new Vector3(rb.velocity.x, 0, rb.velocity.z).magnitude);
        GUILayout.Label("SpeedUp: " + rb.velocity.y);
    }

    public void Update()
    {
        col.material.dynamicFriction = 0f;
        dir = Direction();

        running = (Input.GetKey(KeyCode.LeftShift) && Input.GetAxisRaw("Vertical") > 0.9);
        crouched = (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.C));
        if (Input.GetKeyDown(KeyCode.Space))
        {
            jump = true;
        }

        //Special use
        //if (Input.GetKeyDown(KeyCode.T)) transform.position = new Vector3(0f, 30f, 0f);
        //if (Input.GetKeyDown(KeyCode.X)) rb.velocity = new Vector3(rb.velocity.x, 40f, rb.velocity.z);
        //if (Input.GetKeyDown(KeyCode.V)) rb.AddForce(dir * 20f, ForceMode.VelocityChange);
    }

    public void FixedUpdate()
    {
        if (crouched)
        {
            col.height = Mathf.Max(0.6f, col.height - Time.deltaTime * 10f);
        }
        else
        {
            col.height = Mathf.Min(1.8f, col.height + Time.deltaTime * 10f);
        }

        if (wallStickTimer == 0f && wallBan > 0f)
        {
            bannedGroundNormal = groundNormal;
        }
        else
        {
            bannedGroundNormal = Vector3.zero;
        }

        wallStickTimer = Mathf.Max(wallStickTimer - Time.deltaTime, 0f);
        wallBan = Mathf.Max(wallBan - Time.deltaTime, 0f);

        switch (mode)
        {
            case Mode.Wallruning:
                camCon.SetTilt(WallrunCameraAngle());
                Wallrun(dir, wallSpeed, wallClimbSpeed, wallAccel);
                if (ground.tag != "InfiniteWallrun") wrTimer = Mathf.Max(wrTimer - Time.deltaTime, 0f);
                break;

            case Mode.Walking:
                camCon.SetTilt(0);
                Walk(dir, running ? runSpeed : groundSpeed, grAccel);
                break;

            case Mode.Flying:
                camCon.SetTilt(0);
                AirMove(dir, airSpeed, airAccel);
                break;
        }

        jump = false;
    }



    private Vector3 Direction()
    {
        float hAxis = Input.GetAxisRaw("Horizontal");
        float vAxis = Input.GetAxisRaw("Vertical");

        Vector3 direction = new Vector3(hAxis, 0, vAxis);
        return rb.transform.TransformDirection(direction);
    }



    #region Collisions
    void OnCollisionStay(Collision collision)
    {
        if (collision.contactCount > 0)
        {
            float angle;

            foreach (ContactPoint contact in collision.contacts)
            {
                angle = Vector3.Angle(contact.normal, Vector3.up);
                if (angle < wallFloorBarrier)
                {
                    EnterWalking();
                    grounded = true;
                    groundNormal = contact.normal;
                    ground = contact.otherCollider;
                    return;
                }
            }

            if (VectorToGround().magnitude > 0.2f)
            {
                grounded = false;
            }

            if (grounded == false)
            {
                foreach (ContactPoint contact in collision.contacts)
                {
                    if (contact.otherCollider.tag != "NoWallrun" && contact.otherCollider.tag != "Player" && mode != Mode.Walking)
                    {
                        angle = Vector3.Angle(contact.normal, Vector3.up);
                        if (angle > wallFloorBarrier && angle < 120f)
                        {
                            grounded = true;
                            groundNormal = contact.normal;
                            ground = contact.otherCollider;
                            EnterWallrun();
                            return;
                        }
                    }
                }
            }
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.contactCount == 0)
        {
            EnterFlying();
        }
    }
    #endregion



    #region Entering States
    void EnterWalking()
    {
        if (mode != Mode.Walking && canJump)
        {
            if (mode == Mode.Flying && crouched)
            {
                rb.AddForce(-rb.velocity.normalized, ForceMode.VelocityChange);
            }
            if (rb.velocity.y < -1.2f)
            {
                //camCon.Punch(new Vector2(0, -3f));
            }
            //StartCoroutine(bHopCoroutine(bhopLeniency));
            mode = Mode.Walking;
        }
    }

    void EnterFlying(bool wishFly = false)
    {
        grounded = false;
        if (mode == Mode.Wallruning && VectorToWall().magnitude < wallStickDistance && !wishFly)
        {
            return;
        }
        else if (mode != Mode.Flying)
        {

            wallBan = wallBanTime;
            canDJump = true;
            mode = Mode.Flying;
        }
    }

   public void EnterWallrun()
    {
        if (mode != Mode.Wallruning)
        {
            if (VectorToGround().magnitude > 0.2f && CanRunOnThisWall(bannedGroundNormal) && wallStickTimer == 0f)
            {
                wrTimer = wallRunTime;
                canDJump = true;
                mode = Mode.Wallruning;
            }
            else
            {
                EnterFlying(true);
            }
        }
    }
    #endregion



    #region Movement Types
    void Walk(Vector3 wishDir, float maxSpeed, float acceleration)
    {
        if (jump && canJump)
        {
            Jump();
        }
        else
        {
            //if (crouched) acceleration = 0.5f;
            wishDir = wishDir.normalized;
            Vector3 Speed = new Vector3(rb.velocity.x, 0f, rb.velocity.z);
            if (Speed.magnitude > maxSpeed) acceleration *= Speed.magnitude / maxSpeed;
            Vector3 direction = wishDir * maxSpeed - Speed;

            if (direction.magnitude < 0.5f)
            {
                acceleration *= direction.magnitude / 0.5f;
            }

            direction = direction.normalized * acceleration;
            float magn = direction.magnitude;
            direction = direction.normalized;
            direction *= magn;

            Vector3 slopeCorrection = groundNormal * Physics.gravity.y / groundNormal.y;
            slopeCorrection.y = 0f;
            //if(!crouched)
            direction += slopeCorrection;

            rb.AddForce(direction, ForceMode.Acceleration);
        }
    }

    void AirMove(Vector3 wishDir, float maxSpeed, float acceleration)
    {
        if (jump && !crouched)
        {
            DoubleJump(wishDir);
        }

        if (crouched && rb.velocity.y > -10 && Input.GetKey(KeyCode.Space))
        {
            rb.AddForce(Vector3.down * 20f, ForceMode.Acceleration);
        }

        float projVel = Vector3.Dot(new Vector3(rb.velocity.x, 0f, rb.velocity.z), wishDir); // Vector projection of Current velocity onto accelDir.
        float accelVel = acceleration * Time.deltaTime; // Accelerated velocity in direction of movment

        // If necessary, truncate the accelerated velocity so the vector projection does not exceed max_velocity
        if (projVel + accelVel > maxSpeed)
            accelVel = Mathf.Max(0f, maxSpeed - projVel);

        rb.AddForce(wishDir.normalized * accelVel, ForceMode.VelocityChange);
    }

    void Wallrun(Vector3 wishDir, float maxSpeed, float climbSpeed, float acceleration)
    {
        if (jump)
        {
            //Vertical
            float upForce = Mathf.Clamp(jumpUpSpeed - rb.velocity.y, 0, Mathf.Infinity);
            rb.AddForce(new Vector3(0, upForce, 0), ForceMode.VelocityChange);

            //Horizontal
            Vector3 jumpOffWall = groundNormal.normalized;
            jumpOffWall *= dashSpeed;
            jumpOffWall.y = 0;
            rb.AddForce(jumpOffWall, ForceMode.VelocityChange);
            wrTimer = 0f;
            EnterFlying(true);
        }
        else if (wrTimer == 0f || crouched)
        {
            rb.AddForce(groundNormal * 3f, ForceMode.VelocityChange);
            EnterFlying(true);
        }
        else
        {
            //Horizontal
            Vector3 distance = VectorToWall();
            wishDir = RotateToPlane(wishDir, -distance.normalized);
            wishDir *= maxSpeed;
            wishDir.y = Mathf.Clamp(wishDir.y, -climbSpeed, climbSpeed);
            Vector3 wallrunForce = wishDir - rb.velocity;
            if (wallrunForce.magnitude > 0.2f) wallrunForce = wallrunForce.normalized * acceleration;

            //Vertical
            if (rb.velocity.y < 0f && wishDir.y > 0f) wallrunForce.y = 2f * acceleration;

            //Anti-gravity force
            Vector3 antiGravityForce = -Physics.gravity;
            if (wrTimer < 0.33 * wallRunTime)
            {
                antiGravityForce *= wrTimer / wallRunTime;
                wallrunForce += (Physics.gravity + antiGravityForce);
            }

            //Forces
            rb.AddForce(wallrunForce, ForceMode.Acceleration);
            rb.AddForce(antiGravityForce, ForceMode.Acceleration);
            if (distance.magnitude > wallStickDistance) distance = Vector3.zero;
            rb.AddForce(distance * wallStickiness, ForceMode.Acceleration);
        }
        if (!grounded)
        {
            wallStickTimer = 0.2f;
            EnterFlying();
        }
    }

    void Jump()
    {
        if (mode == Mode.Walking && canJump)
        {
            float upForce = Mathf.Clamp(jumpUpSpeed - rb.velocity.y, 0, Mathf.Infinity);
            rb.AddForce(new Vector3(0, upForce, 0), ForceMode.VelocityChange);
            StartCoroutine(jumpCooldownCoroutine(0.2f));
            EnterFlying(true);
        }
    }

    public void DoubleJump(Vector3 wishDir)
    {
        if (canDJump)
        {
            //Vertical
            float upForce = Mathf.Clamp(jumpUpSpeed - rb.velocity.y, 0, Mathf.Infinity);

            rb.AddForce(new Vector3(0, upForce, 0), ForceMode.VelocityChange);

            //Horizontal
            if (wishDir != Vector3.zero)
            {
                Vector3 horSpeed = new Vector3(rb.velocity.x, 0, rb.velocity.z);
                Vector3 newSpeed = wishDir.normalized;
                float newSpeedMagnitude = dashSpeed;

                if (horSpeed.magnitude > dashSpeed)
                {
                    float dot = Vector3.Dot(wishDir.normalized, horSpeed.normalized);
                    if (dot > 0)
                    {
                        newSpeedMagnitude = dashSpeed + (horSpeed.magnitude - dashSpeed) * dot;
                    }
                    else
                    {
                        newSpeedMagnitude = Mathf.Clamp(dashSpeed * (1 + dot), dashSpeed * (dashSpeed / horSpeed.magnitude), dashSpeed);
                    }
                }

                newSpeed *= newSpeedMagnitude;

                rb.AddForce(newSpeed - horSpeed, ForceMode.VelocityChange);
            }

            canDJump = false;
        }
    }
    #endregion



    #region MathGenious
    Vector2 ClampedAdditionVector(Vector2 a, Vector2 b)
    {
        float k, x, y;
        k = Mathf.Sqrt(Mathf.Pow(a.x, 2) + Mathf.Pow(a.y, 2)) / Mathf.Sqrt(Mathf.Pow(a.x + b.x, 2) + Mathf.Pow(a.y + b.y, 2));
        x = k * (a.x + b.x) - a.x;
        y = k * (a.y + b.y) - a.y;
        return new Vector2(x, y);
    }

    Vector3 RotateToPlane(Vector3 vect, Vector3 normal)
    {
        Vector3 rotDir = Vector3.ProjectOnPlane(normal, Vector3.up);
        Quaternion rotation = Quaternion.AngleAxis(-90f, Vector3.up);
        rotDir = rotation * rotDir;
        float angle = -Vector3.Angle(Vector3.up, normal);
        rotation = Quaternion.AngleAxis(angle, rotDir);
        vect = rotation * vect;
        return vect;
    }

    public float WallrunCameraAngle()
    {
        Vector3 rotDir = Vector3.ProjectOnPlane(groundNormal, Vector3.up);
        Quaternion rotation = Quaternion.AngleAxis(-90f, Vector3.up);
        rotDir = rotation * rotDir;
        float angle = Vector3.SignedAngle(Vector3.up, groundNormal, Quaternion.AngleAxis(90f, rotDir) * groundNormal);
        angle -= 90;
        angle /= 180;
        Vector3 playerDir = transform.forward;
        Vector3 normal = new Vector3(groundNormal.x, 0, groundNormal.z);

        return Vector3.Cross(playerDir, normal).y * angle;
    }

    bool CanRunOnThisWall(Vector3 normal)
    {
        if (Vector3.Angle(normal, groundNormal) > 10 || wallBan == 0f)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    Vector3 VectorToWall()
    {
        Vector3 direction;
        Vector3 position = transform.position + Vector3.up * col.height / 2f;
        RaycastHit hit;
        if (Physics.Raycast(position, -groundNormal, out hit, wallStickDistance) && Vector3.Angle(groundNormal, hit.normal) < 70)
        {
            groundNormal = hit.normal;
            direction = hit.point - position;
            return direction;
        }
        else
        {
            return Vector3.positiveInfinity;
        }
    }

    Vector3 VectorToGround()
    {
        Vector3 position = transform.position;
        RaycastHit hit;
        if (Physics.Raycast(position, Vector3.down, out hit, wallStickDistance))
        {
            return hit.point - position;
        }
        else
        {
            return Vector3.positiveInfinity;
        }
    }
    #endregion



    #region Coroutines
    IEnumerator jumpCooldownCoroutine(float time)
    {
        canJump = false;
        yield return new WaitForSeconds(time);
        canJump = true;
    }
    #endregion
}
