
using UnityEngine;

namespace Taijj.SampleCode
{
	public static class AnimatorHashes
	{
		public static readonly int IDLE = Animator.StringToHash("Idle");
		public static readonly int FLINCH = Animator.StringToHash("Flinch");
		public static readonly int DIE = Animator.StringToHash("Die");

		public static readonly int WALK = Animator.StringToHash("Walk");
		public static readonly int RUN = Animator.StringToHash("Run");		

		public static readonly int JUMP = Animator.StringToHash("Jump");
		public static readonly int FALL = Animator.StringToHash("Fall");

		public static readonly int ATTACK = Animator.StringToHash("Attack");
		public static readonly int THROW = Animator.StringToHash("Throw");
		public static readonly int IS_ATTACKING = Animator.StringToHash("IsAttacking");

		public static readonly int CYCLE_OFFSET = Animator.StringToHash("CycleOffset");
	}
}