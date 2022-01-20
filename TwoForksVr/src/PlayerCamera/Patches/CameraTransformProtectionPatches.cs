using System;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using TwoForksVr.Helpers;
using TwoForksVr.PlayerBody;
using UnityEngine;

// Even though Unity prevents moving / rotating a VR camera directly, the transform values still change until the next update.
// We need to disable any code that tries to move the camera directly, so that the transform values remain "clean".
// All these patches try to disable any game code that would otherwise mess with the camera's transform values.
namespace TwoForksVr.PlayerCamera.Patches
{
    [HarmonyPatch]
    public class CameraTransformProtectionPatches
    {
        [HarmonyPrefix]
        [HarmonyPatch(typeof(vgCameraController), nameof(vgCameraController.ApplyPostAnimationTransform))]
        [HarmonyPatch(typeof(vgCameraController), nameof(vgCameraController.UpdateFOV))]
        [HarmonyPatch(typeof(vgCameraController), nameof(vgCameraController.UpdateClipPlaneOffset))]
        // [HarmonyPatch(typeof(vgCameraTargetSource), nameof(vgCameraTargetSource.UpdateLookAt))]
        // [HarmonyPatch(typeof(vgCameraTargetSource), nameof(vgCameraTargetSource.UpdateAnimation))]
        // [HarmonyPatch(typeof(vgCameraTargetSource), nameof(vgCameraTargetSource.UpdateFromInput))]
        private static bool DisableCameraModifications()
        {
            return false;
        }

    [HarmonyPrefix]
    [HarmonyPatch(typeof(vgCameraController), nameof(vgCameraController.UpdateCameraStack))]
	private static bool UpdateCameraStack(vgCameraController __instance)
	{
		return false;
		if (__instance.debugCameraModes)
		{
			if (__instance.debugText == null)
			{
				GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(new GameObject(), new Vector3(0f, 1f, 0.5f), Quaternion.identity);
				__instance.debugText = gameObject.AddComponent<GUIText>();
			}
			__instance.debugText.text = string.Empty;
		}
		vgInputHandler instance = vgInputHandler.Instance;
		__instance.bodyLimits.CopyLimit(__instance.defaultBodyLimit);
		__instance.headLimits.CopyLimit(__instance.defaultHeadLimit);
		__instance.eyeLimits.CopyLimit(__instance.defaultEyeLimit);
		CameraModePriority cameraModePriority = CameraModePriority.cmpNormal;
		__instance.bodyLocalOffset = Vector3.zero;
		__instance.headLocalOffset = Vector3.zero;
		__instance.eyeLocalOffset = Vector3.zero;
		__instance.bodyOffsetToApplyToModel = Vector3.zero;
		__instance.lastFrameRotation = Vector3.zero;
		vgCameraOffsetByAngle vgCameraOffsetByAngle = new vgCameraOffsetByAngle();
		bool flag = false;
		bool flag2 = true;
		LinkedListNode<vgCameraModeInstance> linkedListNode;
		for (linkedListNode = __instance.cameraModeStack.First; linkedListNode != null; linkedListNode = linkedListNode.Next)
		{
			vgCameraModeInstance value = linkedListNode.Value;
			value.UpdateBlendState();
		}
		ControlMetaphor controlMetaphor = ControlMetaphor.cmEye;
		linkedListNode = __instance.cameraModeStack.First;
		while (linkedListNode != null)
		{
			vgCameraModeInstance value2 = linkedListNode.Value;
			vgCameraModeObject sourceMode = value2.sourceMode;
			if (instance != null)
			{
				value2.UpdateCrossFade();
				value2.UpdateMode(instance);
				if (__instance.debugCameraModes)
				{
					string str = string.Empty;
					value2.DebugDraw(ref str);
					if (value2.currentBlendState == BlendState.bsStarting)
					{
						str = "<color=#00ff00ff>" + str + "</color>";
					}
					else if (value2.currentBlendState == BlendState.bsEnding)
					{
						str = "<color=#ff0000ff>" + str + "</color>";
					}
					GUIText guitext = __instance.debugText;
					guitext.text = guitext.text + str + "\n";
				}
			}
			if (value2.crossFadeModes != null && value2.crossFadeModes.Count > 0)
			{
				for (int i = 0; i < value2.crossFadeModes.Count; i++)
				{
					if (value2.crossFadeModes[i].sourceMode.rotateModel && value2.crossFadeModes[i].sourceMode.metaphor == ControlMetaphor.cmBody)
					{
						vgCameraModeInstance vgCameraModeInstance = value2;
						vgCameraModeInstance.localOffset.x = vgCameraModeInstance.localOffset.x - __instance.appliedBodyYaw;
					}
				}
			}
			if (sourceMode.priority >= cameraModePriority && cameraModePriority != CameraModePriority.cmpExclusive)
			{
				if (sourceMode.priority > cameraModePriority)
				{
					__instance.bodyLocalDelta = (__instance.eyeLocalDelta = (__instance.headLocalDelta = Vector3.zero));
					__instance.bodyOffsetToApplyToModel = (__instance.bodyLocalOffset = (__instance.eyeLocalOffset = (__instance.headLocalOffset = Vector3.zero)));
					cameraModePriority = sourceMode.priority;
				}
				ControlMetaphor metaphor = sourceMode.metaphor;
				if (metaphor != ControlMetaphor.cmBody)
				{
					if (metaphor != ControlMetaphor.cmHead)
					{
						if (metaphor == ControlMetaphor.cmEye)
						{
							__instance.ApplyMode(value2, __instance.eyeLimits, ref __instance.eyeLocalDelta, ref __instance.eyeLocalOffset, vgCameraOffsetByAngle);
						}
					}
					else
					{
						__instance.ApplyMode(value2, __instance.headLimits, ref __instance.headLocalDelta, ref __instance.headLocalOffset, vgCameraOffsetByAngle);
					}
				}
				else
				{
					if (sourceMode.rotateModel)
					{
						vgCameraModeInstance vgCameraModeInstance2 = value2;
						vgCameraModeInstance2.localOffset.x = vgCameraModeInstance2.localOffset.x - __instance.appliedBodyYaw;
					}
					__instance.ApplyMode(value2, __instance.bodyLimits, ref __instance.bodyLocalDelta, ref __instance.bodyLocalOffset, vgCameraOffsetByAngle);
					if (sourceMode.rotateModel)
					{
						__instance.bodyOffsetToApplyToModel += __instance.bodyLocalOffset;
					}
				}
				if (sourceMode.targetSource.sourceType == TargetSourceType.tsMecanim)
				{
					flag = true;
				}
				if (sourceMode.targetSource.sourceType == TargetSourceType.tsMouseInput)
				{
					__instance.lastFrameRotation += value2.localOffset;
				}
				if (sourceMode.targetSource.sourceType == TargetSourceType.tsAnimated)
				{
					controlMetaphor = sourceMode.metaphor;
				}
			}
			if (value2.currentBlendState != BlendState.bsEnding)
			{
				flag2 = false;
			}
			LinkedListNode<vgCameraModeInstance> linkedListNode2 = linkedListNode;
			linkedListNode = linkedListNode.Next;
			if (value2.DeleteMe)
			{
				__instance.RecycleInstance(linkedListNode2.Value);
				__instance.cameraModeStack.Remove(linkedListNode2);
			}
		}
		vgMath.ApplyLimit(ref __instance.eyeLocalOffset, ref __instance.eyeLimits);
		vgMath.ApplyLimit(ref __instance.headLocalOffset, ref __instance.headLimits);
		vgMath.ApplyLimit(ref __instance.bodyLocalOffset, ref __instance.bodyLimits);
		Quaternion quaternion = __instance.CalculateCameraRotation(__instance.bodyLocalOffset);
		Quaternion quaternion2 = __instance.CalculateCameraRotation(__instance.headLocalOffset);
		Quaternion rhs = __instance.CalculateCameraRotation(__instance.eyeLocalOffset);
		Quaternion rhs2;
		if (controlMetaphor == ControlMetaphor.cmBody)
		{
			rhs2 = quaternion;
			__instance.postAnimationTransform = quaternion2 * rhs;
		}
		else if (controlMetaphor == ControlMetaphor.cmHead)
		{
			rhs2 = quaternion * quaternion2;
			__instance.postAnimationTransform = rhs;
		}
		else
		{
			rhs2 = quaternion * quaternion2 * rhs;
			__instance.postAnimationTransform = Quaternion.identity;
		}
		if (flag)
		{
			Vector3 eulerAngles = __instance.eye.rotation.eulerAngles;
			Vector3 eulerAngles2 = new Vector3(eulerAngles.z, eulerAngles.y - 90f, eulerAngles.x);
			Quaternion lhs = default(Quaternion);
			lhs.eulerAngles = eulerAngles2;
			__instance.transform.rotation = lhs * rhs2;
		}
		else
		{
			__instance.transform.rotation *= rhs2;
			__instance.transform.rotation =
				Quaternion.LookRotation(Vector3.ProjectOnPlane(__instance.transform.forward, Vector3.up), Vector3.up);
			// __instance.transform.up = Vector3.up;
		}
		float num = __instance.transform.rotation.eulerAngles.x;
		num = vgMath.WrapAngle(num);
		float num2 = rhs2.eulerAngles.y;
		num2 = vgMath.WrapAngle(num2);
		__instance.cameraOffset = vgCameraOffsetByAngle.GetOffset(num, num2);
		
		// vgCameraController.UpdateLookAngle(-1f * num);
		// The above line is replaced with this thing:
		var eventDelegate = (MulticastDelegate) typeof(vgCameraController).GetField(nameof(vgCameraController.UpdateLookAngle), BindingFlags.Static | BindingFlags.NonPublic).GetValue(null);
		eventDelegate.DynamicInvoke(-1f * num);
		if (flag2)
		{
			__instance.PushCameraModes(__instance.defaultCameraTuning, true);
		}

		return false;
	}

        [HarmonyPrefix]
        [HarmonyPatch(typeof(vgCameraController), nameof(vgCameraController.LateUpdate))]
        private static bool DisableCameraLateUpdate()
        {
            // If I always disable this method, it will break the camera position.
            // Since it was only broken while paused, I'm disabling it only in that scenario.
            return Time.timeScale != 0;
        }
        
        [HarmonyPrefix]
        [HarmonyPatch(typeof(vgCameraController), nameof(vgCameraController.UpdatePosition))]
        private static bool DisableCameraRotation(vgCameraController __instance)
        {
            __instance.transform.position = __instance.eyeTransform.position;
            
            return false;
        }
    }
}