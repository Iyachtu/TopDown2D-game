using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

enum PlayerStateMode {LOCOMOTION, ROLL, SPRINT }
public class PlayerStateMachine : MonoBehaviour
{
    [SerializeField] float _rollDuration = 0.2f;
    [SerializeField] float _movespeed = 0.3f;
    [SerializeField] float _runspeed = 0.6f;
    float _rollspeed = 0.5f;
    [SerializeField] AnimationCurve _rollCurve;
    private PlayerStateMode _currentState;
    private Animator _pAnimator;
    private float _endRollTime;
    Vector2 _direction;
    Rigidbody2D _rb;
    float _currentspeed;
    float _rollcount = 0;
    [SerializeField] float _rollMulti = 300;

    private void Awake()
    {
        _pAnimator = GetComponentInChildren<Animator>();
        _rb = GetComponent<Rigidbody2D>();
    }
    // Start is called before the first frame update
    void Start()
    {
        TransitionToState(PlayerStateMode.LOCOMOTION);
        //playerActionControls.Land.Roll.performed += _ => Roll();
        //playerActionControls.Land.Run.pressed += _ => Run();
    }

    // Update is called once per frame
    void Update()
    {
        OnStateUpdate();
        SetInput();

        //_currentposition = transform.position;
        //_currentposition.x += movementInput * _movespeed * Time.deltaTime;
        //transform.position = _currentposition;
    }


    private void FixedUpdate()
    {
        _rb.velocity = _direction.normalized * _currentspeed *Time.fixedDeltaTime;
    }

    private void SetInput()
    {
        _direction.x = Input.GetAxisRaw("Horizontal");
        _direction.y = Input.GetAxisRaw("Vertical");
    }
    void OnStateEnter ()
    {
        switch (_currentState)
        {
            case PlayerStateMode.LOCOMOTION:
                break;
            case PlayerStateMode.ROLL:
                _pAnimator.SetBool("isRolling", true);
                _endRollTime = Time.timeSinceLevelLoad + _rollDuration;
                break;
            case PlayerStateMode.SPRINT:
                _pAnimator.SetBool("isSprinting", true);
                break;
            default:
                break;
        }
    }
    void OnStateExit()
    {
        switch (_currentState)
        {
            case PlayerStateMode.LOCOMOTION:
                break;
            case PlayerStateMode.ROLL:
                _pAnimator.SetBool("isRolling", false);
                break;
            case PlayerStateMode.SPRINT:
                _pAnimator.SetBool("isSprinting", false);
                break;
            default:
                break;
        }
    }
    
    void OnStateUpdate()
    {
        switch (_currentState)
        {
            case PlayerStateMode.LOCOMOTION:
                _pAnimator.SetFloat("DirX", _direction.x);
                _pAnimator.SetFloat("DirY", _direction.y);
                _currentspeed = _movespeed;

                if (Input.GetButtonDown("Fire3")) { TransitionToState(PlayerStateMode.ROLL); }
                break;
            case PlayerStateMode.ROLL:
                _rollcount += Time.deltaTime;
                _rollspeed = _rollCurve.Evaluate(_rollcount/ _rollDuration) * _rollMulti;
                _currentspeed= _rollspeed;

                if (Time.timeSinceLevelLoad>_endRollTime)
                {
                    if (Input.GetButton("Fire3")) TransitionToState(PlayerStateMode.SPRINT);
                    else TransitionToState(PlayerStateMode.LOCOMOTION);
                }
                break;
            case PlayerStateMode.SPRINT:
                _currentspeed = _runspeed;
                _pAnimator.SetFloat("DirX", _direction.x);
                _pAnimator.SetFloat("DirY", _direction.y);
                if (!Input.GetButton("Fire3")) TransitionToState(PlayerStateMode.LOCOMOTION);
                break;
            default:
                break;
        }
    }

    void TransitionToState(PlayerStateMode toState)
    {
        OnStateExit();
        _currentState = toState;
        OnStateEnter();
    }
}
