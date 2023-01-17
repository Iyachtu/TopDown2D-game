using System.Collections;
using System.Collections.Generic;
using UnityEngine;

enum PlayerStateMode {LOCOMOTION, ROLL, SPRINT }
public class PlayerStateMachine : MonoBehaviour
{
    private PlayerStateMode _currentState;
    private Animator _pAnimator;
    [SerializeField] float _rollDuration = 0.1f;
    private float _endRollTime;

    private void Awake()
    {
        _pAnimator = GetComponentInChildren<Animator>();
    }
    // Start is called before the first frame update
    void Start()
    {
        TransitionToState(PlayerStateMode.LOCOMOTION);
    }

    // Update is called once per frame
    void Update()
    {
        OnStateUpdate();
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
                _pAnimator.SetFloat("DirX", Input.GetAxis("Horizontal"));
                _pAnimator.SetFloat("DirY", Input.GetAxis("Vertical"));

                if (Input.GetButtonDown("Fire3")) { TransitionToState(PlayerStateMode.ROLL); }
                break;
            case PlayerStateMode.ROLL:
                if (Time.timeSinceLevelLoad>_endRollTime)
                {
                    if (Input.GetButton("Fire3")) TransitionToState(PlayerStateMode.SPRINT);
                    else TransitionToState(PlayerStateMode.LOCOMOTION);
                }
                break;
            case PlayerStateMode.SPRINT:
                _pAnimator.SetFloat("DirX", Input.GetAxis("Horizontal"));
                _pAnimator.SetFloat("DirY", Input.GetAxis("Vertical"));
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
