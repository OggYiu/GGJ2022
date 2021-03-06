// (c) Copyright HutongGames, LLC 2010-2013. All rights reserved.

using UnityEngine;

namespace HutongGames.PlayMaker.Actions
{
	[ActionCategory(ActionCategory.GameObject)]
	[Tooltip("Gets a Game Object's Transform and stores it in an Object Variable.")]
	public class GetTransform : FsmStateAction
	{
		[RequiredField]
        [Tooltip("The Game Object.")]
		public FsmGameObject gameObject;
		
		[RequiredField]
		[UIHint(UIHint.Variable)]
		[ObjectType(typeof(Transform))]
        [Tooltip("Store the GameObject's Transform in an Object variable of type UnityEngine.Transform.")]
		public FsmObject storeTransform;
		
        [Tooltip("Repeat every frame.")]
		public bool everyFrame;

		public override void Reset()
		{
			gameObject = new FsmGameObject { UseVariable = true};
			storeTransform = null;
			everyFrame = false;
		}

		public override void OnEnter()
		{
			DoGetGameObjectName();
			
			if (!everyFrame)
			{
				Finish();
			}
		}
		
		public override void OnUpdate()
		{
			DoGetGameObjectName();
		}
		
		void DoGetGameObjectName()
		{
			var go = gameObject.Value;

			storeTransform.Value = go != null ? go.transform : null;
		}
	}
}