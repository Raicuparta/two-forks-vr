using System.Collections.Generic;

namespace TwoForksVr.Input
{
	public class VrAxisData : vgKeyData
	{
		public VrAxisData(string name, float deadzone = 0.5f)
		{
			names = new List<string>(1);
			names.Add(name);
			buttonDeadzone = deadzone;
		}

		public override void Update()
		{
			axisValueLastFrame = axisValue;
			string axisName = names[0];
			axisValue = UnityEngine.Input.GetAxisRaw(axisName);
		}

		public override bool Released()
		{
			return CheckThreshold(buttonDeadzone) == ThresholdResult.CrossedBelow;
		}

		public override bool Pressed()
		{
			return CheckThreshold(buttonDeadzone) == ThresholdResult.CrossedAbove;
		}

		public override bool IsHeld()
		{
			return GetAxisValue() >= buttonDeadzone;
		}

		public override bool ShouldHold(float duration)
		{
			return false;
		}

		public override float GetAxisValue()
		{
			return axisValue;
		}

		public override float GetPreviousAxisValue()
		{
			return axisValueLastFrame;
		}

		public override vgKeyCodeToVirtualKey.KeyType GetKeyType()
		{
			return vgKeyCodeToVirtualKey.KeyType.Axis;
		}

		private float axisValue;

		private float axisValueLastFrame;

		private float buttonDeadzone = 0.5f;
	}
}
