using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerController : MonoBehaviour {
    SpriteRenderer spriteRenderer;
    BoxCollider2D coll;
    Rigidbody2D body;
    Transform eye1;
    Transform eye2;
    FixedTimer jumpStrengthTimer; int minJumpFrames = 3; int maxJumpFrames = 13;
    FixedTimer landingJumpTimer; int landingJumpFrames = 5;
    FixedTimer dashingJumpTimer; int dashingJumpFrames = 5;
    FixedTimer coyoteTimer; int coyoteFrames = 4;
    FixedTimer dashTimer; int dashFrames = 10;
    FixedTimer landingDashTimer; int landingDashFrames = 5;
    float weight;
    int _facing = 1;
    int _looking = 0;
    float maxSpeed = 8f;
    float maxFallSpeedRatio = 3f;
    float accelerationMagnitude = 240f;
    float jumpMagnitude = 360f;
    float dashMagnitude = 400f;
    float dashSpeedRatio = 4f;
    bool attackButtonDown = false;
    bool jumpButtonDown = false;
    bool dashButtonDown = false;
    bool hasDash = false;

    public int facing {
        get { return _facing; }
    }
    public int looking {
        get { return _looking; }
    }

    void Awake() {
        spriteRenderer = GetComponent<SpriteRenderer>();
        coll = GetComponent<BoxCollider2D>();
        body = GetComponent<Rigidbody2D>();

        weight = Mathf.Abs(body.gravityScale * Physics2D.gravity.y * body.mass);

        eye1 = transform.GetChild(0);
        eye2 = transform.GetChild(1);


        jumpStrengthTimer = gameObject.AddComponent<FixedTimer>(); jumpStrengthTimer.Init(maxJumpFrames);
        landingJumpTimer = gameObject.AddComponent<FixedTimer>(); landingJumpTimer.Init(landingJumpFrames);
        dashingJumpTimer = gameObject.AddComponent<FixedTimer>(); dashingJumpTimer.Init(dashingJumpFrames);
        coyoteTimer = gameObject.AddComponent<FixedTimer>(); coyoteTimer.Init(coyoteFrames);
        dashTimer = gameObject.AddComponent<FixedTimer>(); dashTimer.Init(dashFrames);
        landingDashTimer = gameObject.AddComponent<FixedTimer>(); landingDashTimer.Init(landingDashFrames);
    }
    void Update() {
        if (Input.GetButtonDown("Attack")) attackButtonDown = true;
        if (Input.GetButtonDown("Jump")) jumpButtonDown = true;
        if (Input.GetButtonDown("Dash")) dashButtonDown = true;
    }
    void FixedUpdate() {
        /// Facing
        if (!dashTimer.Running) {
            if (Input.GetAxis("Horizontal") > 0) {
                _facing = 1;
            }
            else if (Input.GetAxis("Horizontal") < 0) {
                _facing = -1;
            }

            if (Input.GetAxis("Vertical") > 0) {
                _looking = 1;
            }
            else if (Input.GetAxis("Vertical") < 0) {
                _looking = -1;            
            }
            else {
                _looking = 0;
            }
        }
        else {
            _looking = 0;
        }
        eye1.position = transform.position + new Vector3(.1f*_facing, .15f*_looking, eye1.position.z);
        eye2.position = transform.position + new Vector3(.35f*_facing, .15f*_looking, eye1.position.z);

        ///
        if (IsGrounded()) {
            coyoteTimer.Run();
        }
        if (coyoteTimer.Running) {
            hasDash = true;
        }

        /// Dash
        if ( dashButtonDown && !hasDash ) {
            landingDashTimer.Run();
        }
        if ( (dashButtonDown || landingDashTimer.Running) && !dashTimer.Running && hasDash ) {
            body.gravityScale = 0;
            body.velocity = Vector2.zero;
            dashTimer.Run();
            jumpStrengthTimer.Stop();
            landingDashTimer.Stop();
        }
        if (dashTimer.Running) {
            float desiredSpeed = maxSpeed * dashSpeedRatio * _facing;
            float deltaSpeed = (desiredSpeed - body.velocity.x) / (maxSpeed * dashSpeedRatio);
            body.AddForce(dashMagnitude * deltaSpeed * body.mass * Vector2.right);
        }
        if (dashTimer.JustFinished) {
            body.gravityScale = 8;
            hasDash = false;
        }

        /// Horizontal Movement
        if (!dashTimer.Running) {
            float desiredSpeed = maxSpeed * Input.GetAxis("Horizontal");
            float deltaSpeed = (desiredSpeed - body.velocity.x) / maxSpeed;
            body.AddForce(accelerationMagnitude * deltaSpeed * body.mass * Vector2.right);
        }

        /// Jump
        if ( jumpButtonDown && !(IsGrounded() || coyoteTimer.Running) ) {
            landingJumpTimer.Run();
        }
        if ( jumpButtonDown && dashTimer.Running ) {
            dashingJumpTimer.Run();
        }
        if (!dashTimer.Running) {
            if ( (jumpButtonDown || landingJumpTimer.Running || dashingJumpTimer.Running) && (IsGrounded() || coyoteTimer.Running) && !jumpStrengthTimer.Running ) {
                body.velocity = new Vector2(body.velocity.x, 0);
                jumpStrengthTimer.Run();
                landingJumpTimer.Stop();
                dashingJumpTimer.Stop();
            }
            if ( (Input.GetButton("Jump") || jumpStrengthTimer.Elapsed < minJumpFrames) && jumpStrengthTimer.Running ) {
                body.AddForce(jumpMagnitude * Mathf.Pow(((float)jumpStrengthTimer.Remaining / (float)maxJumpFrames), 2f) * body.mass * Vector2.up);
            }
        }
        
        /// Falling Drag
        if (body.velocity.y <= -.9f * maxFallSpeedRatio * maxSpeed) {
            body.AddForce( Mathf.Abs(body.velocity.y) / (maxFallSpeedRatio * maxSpeed) * weight * Vector2.up );
        }

        ///
        if (attackButtonDown) attackButtonDown = false;
        if (jumpButtonDown) jumpButtonDown = false;
        if (dashButtonDown) dashButtonDown = false;
    }
    bool IsGrounded() {
        RaycastHit2D raycastHit2D = Physics2D.BoxCast(coll.bounds.center, coll.bounds.size, 0, Vector2.down, .04f, LayerMask.GetMask("Wall"));

        Color debugColor = (raycastHit2D.collider != null) ? Color.green : Color.red;

        Vector2 c = coll.bounds.center;
        Vector2 pp = new Vector2(coll.bounds.extents.x, coll.bounds.extents.y);
        Vector2 pn = new Vector2(coll.bounds.extents.x, -coll.bounds.extents.y);
        
        Debug.DrawLine(c + pp, c + pn, debugColor);
        Debug.DrawLine(c - pp, c - pn, debugColor);
        Debug.DrawLine(c + pp, c - pn, debugColor);
        Debug.DrawLine(c - pp, c + pn, debugColor);

        return (raycastHit2D.collider != null);
    }
    float SignedPow(float b, float p) {
        return Mathf.Sign(b) * Mathf.Abs(Mathf.Pow(b, p));
    }
}