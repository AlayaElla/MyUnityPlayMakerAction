using UnityEngine;
using System.Collections;

namespace HutongGames.PlayMaker.Actions
{
    [ActionCategory(ActionCategory.GameLogic)]
    public class CreateByPool : FsmStateAction
    {
        public FsmOwnerDefault Root;
        public FsmString Path;
        [Tooltip("Optional Spawn Point.")]
        public FsmGameObject spawnPoint;
        [Tooltip("Position. If a Spawn Point is defined, this is used as a local offset from the Spawn Point position.")]
        public FsmVector3 position;
        [Tooltip("Rotation. NOTE: Overrides the rotation of the Spawn Point.")]
        public FsmVector3 rotation;

        public FsmGameObject Result;

        public override void Reset()
        {
            Root = null;
            Path = null;
            spawnPoint = null;
            position = new FsmVector3 { UseVariable = true };
            rotation = new FsmVector3 { UseVariable = true };
            Result = null;
        }

        public override void OnEnter()
        {
            GameObject go = Fsm.GetOwnerDefaultTarget(Root);
            //if (go == null) return;

            Result.Value = itemPool.Instance.CreatePool(Path.Value, go);
            var spawnPosition = Vector3.zero;
            var spawnRotation = Vector3.zero;

            if (spawnPoint.Value != null)
            {
                spawnPosition = spawnPoint.Value.transform.position;

                if (!position.IsNone)
                {
                    spawnPosition += position.Value;
                }

                spawnRotation = !rotation.IsNone ? rotation.Value : spawnPoint.Value.transform.eulerAngles;
            }
            else
            {
                if (!position.IsNone)
                {
                    spawnPosition = position.Value;
                }

                if (!rotation.IsNone)
                {
                    spawnRotation = rotation.Value;
                }
            }
            Result.Value.transform.position = spawnPosition;
            Result.Value.transform.rotation = Quaternion.Euler(spawnRotation);
            Finish();
        }
    }
}


