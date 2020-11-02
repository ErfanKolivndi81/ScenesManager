using System;
namespace IRK.Unity.ScenesManager
{
	/// <summary>
	/// Used when you want the <see cref="TransitionEffect.duration"/> to be controlled by you
	/// (Useful when <see cref="ScenesManagerCore.durationTransitionEffectsAll"/> is true).
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false, Inherited = false)]
	public class CustomDurationAttribute : Attribute { }

	/// <summary>
	/// Used when you want the  <see cref="TransitionEffect.sleep"/> to be controlled by you
	/// (Useful when <see cref="ScenesManagerCore.sleepTransitionEffectsAll"/> is true).
	/// </summary>
	[AttributeUsage(AttributeTargets.Class, AllowMultiple = false,Inherited = false)]
	public class CustomSleepAttribute : Attribute { }
}
