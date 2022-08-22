using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.GameObject)]
	[Tooltip("Destroys a Game Object.")]
	public class RecycleToPool : FsmStateAction
	{
		[RequiredField]
		[Tooltip("The GameObject to destroy.")]
		public FsmOwnerDefault gameObject;


		public override void Reset()
		{
			gameObject = null;
		}

		public override void OnEnter()
		{
			var go = Fsm.GetOwnerDefaultTarget(gameObject);

			if (go != null)
			{
				itemPool.Instance.RecycleObj(go);
			}
			
			Finish();
		}
	}
}