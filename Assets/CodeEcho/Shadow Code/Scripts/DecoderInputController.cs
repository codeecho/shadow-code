using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class DecoderInputController : MonoBehaviour
{
    void OnDecode(InputValue value)
    {
        if (value.isPressed)
        {
            EventManager.Trigger("DecoderEnabled");
        }
        else
        {
            EventManager.Trigger("DecoderDisabled");
        }
    }
}
