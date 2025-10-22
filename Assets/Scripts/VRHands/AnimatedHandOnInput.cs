using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class AnimatedHandOnInput : MonoBehaviour
{

	public InputActionProperty pinchAnimationAction;
	public InputActionProperty gripAnimationAction;

	public Animator handAnimator;

	// Update is called once per frame
	void Update()
	{
		handAnimator.SetFloat("Trigger", pinchAnimationAction.action.ReadValue<float>());

		handAnimator.SetFloat("Grip", gripAnimationAction.action.ReadValue<float>());
	}
}
