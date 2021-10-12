using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class PlayerAnimatorManager : MonoBehaviourPun
{
    #region Private Fiels

    [SerializeField] private float directionDampTime = 0.25f;
    private Animator _animator;
    private static readonly int Direction = Animator.StringToHash("Direction");
    private static readonly int Speed = Animator.StringToHash("Speed");
    private static readonly int Jump = Animator.StringToHash("Jump");

    #endregion

    // Start is called before the first frame update
    void Start()
    {
        _animator = GetComponent<Animator>();
        if (!_animator)
        {
            Debug.LogError("PlayerAnimatorManager is Missing Animator Component", this);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!photonView.IsMine && PhotonNetwork.IsConnected)
        {
            return;
        }
        
        if (_animator)
        {
            UpdateWithAnimator();
        }
    }

    private void UpdateWithAnimator()
    {
        // deal with Jumping
        AnimatorStateInfo stateInfo = _animator.GetCurrentAnimatorStateInfo(0);
        //only allow jumping if we are running.
        if (stateInfo.IsName("Base Layer.Run"))
        {
            //When using trigger parameter
            if (Input.GetButtonDown("Fire2"))
            {
                _animator.SetTrigger(Jump);
            }
        }

        // deal with running
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        if (v < 0)
        {
            v = 0;
        }

        _animator.SetFloat(Speed, h*h + v*v);
        _animator.SetFloat(Direction, h, directionDampTime, Time.deltaTime);
    }
}