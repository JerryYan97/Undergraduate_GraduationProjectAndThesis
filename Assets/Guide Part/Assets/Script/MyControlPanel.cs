﻿using System;
using System.Collections.Generic;
using UnityEngine;

public class MyControlPanel : MonoBehaviour
{
    [SerializeField]
    KeyCode SpeedUp = KeyCode.Space;
    [SerializeField]
    KeyCode SpeedDown = KeyCode.LeftShift;
    [SerializeField]
    KeyCode Forward = KeyCode.W;
    [SerializeField]
    KeyCode Back = KeyCode.S;
    [SerializeField]
    KeyCode Left = KeyCode.A;
    [SerializeField]
    KeyCode Right = KeyCode.D;
    [SerializeField]
    KeyCode TurnLeft = KeyCode.Q;
    [SerializeField]
    KeyCode TurnRight = KeyCode.E;
    [SerializeField]
    KeyCode MusicOffOn = KeyCode.M;

    private KeyCode[] keyCodes;

    public Action<PressedKeyCode[]> KeyPressed;
    private void Awake()
    {
        keyCodes = new[] {
                            SpeedUp,
                            SpeedDown,
                            Forward,
                            Back,
                            Left,
                            Right,
                            TurnLeft,
                            TurnRight
                        };

    }

    void FixedUpdate()
    {
        var pressedKeyCode = new List<PressedKeyCode>();
        for (int index = 0; index < keyCodes.Length; index++)
        {
            var keyCode = keyCodes[index];
            if (Input.GetKey(keyCode))
                pressedKeyCode.Add((PressedKeyCode)index);
        }

        if (KeyPressed != null)
            KeyPressed(pressedKeyCode.ToArray());
    }
}