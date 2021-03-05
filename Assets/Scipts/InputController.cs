using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

public class InputController : MonoBehaviour
{
    private Joystick joystick;
    public string inputThrottleAxis = "Vertical";

    public float ThrottleInput;
    public float SteerInput;

    private void Start()
    {
        joystick = GameObject.FindGameObjectWithTag("Joystick").GetComponent<Joystick>();
    }
    void Update()
    {
        SteerInput = joystick.Horizontal;
        SteerInput = Input.GetAxisRaw("Horizontal");
        ThrottleInput = CrossPlatformInputManager.GetAxisRaw(inputThrottleAxis);
        ThrottleInput = Input.GetAxisRaw("Vertical");
    }
}
