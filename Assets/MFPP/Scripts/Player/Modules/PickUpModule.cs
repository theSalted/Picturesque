using UnityEngine;

namespace MFPP.Modules
{
    [HelpURL("https://ashkoredracson.github.io/MFPP/#pick-up-module")]
    public class PickUpModule : PlayerModule
    {
        /// <summary>
        /// The maximum pickup distance.
        /// </summary>
        [Space]
        [Tooltip("The maximum pickup distance.")]
        public float MaxPickupDistance = 2f;
        /// <summary>
        /// The maximum pickup mass.
        /// </summary>
        [Tooltip("The maximum pickup mass.")]
        public float MaxPickupMass = 5f;
        /// <summary>
        /// The force of the picked up object movement.
        /// </summary>
        [Tooltip("The force of the picked up object movement.")]
        public float MoveForce = 5f;
        /// <summary>
        /// Pick up button.
        /// </summary>
        [Space]
        [Tooltip("Pick up button.")]
        public string PickUpButton = "Pick Up";

        private Rigidbody target;
        private Quaternion originalRotation;

        public override void AfterUpdate()
        {
            if (Input.GetButtonDown(PickUpButton)) // If pick up button was pressed
            {
                if (target != null) // If we already have a target rigidbody, set it to null, thus dropping/throwing it.
                {
                    target = null;
                }
                else
                {
                    Ray r = new Ray(Camera.transform.position, Camera.transform.forward);
                    RaycastHit hit;
                    if (Physics.Raycast(r, out hit, MaxPickupDistance)) // Otherwise, if target was null, shoot a ray where we are aiming.
                    {
                        Rigidbody body = hit.collider.attachedRigidbody;
                        if (body != null && !body.isKinematic && body.mass <= MaxPickupMass) // Retreive the rigidbody and assure that it is not kinematic.
                        {
                            target = body; // Set the target
                            originalRotation = target.rotation;
                        }
                    }
                }
            }

            if (target != null) // If target is not null, move the target.
            {
                Vector3 floatingPosition = Camera.transform.position + Camera.transform.forward * MaxPickupDistance;

                var delta = (Player.Main.Camera.transform.rotation * Quaternion.Inverse(originalRotation)) * Quaternion.Inverse(target.rotation);

                float angle; Vector3 axis;
                delta.ToAngleAxis(out angle, out axis);        

                if (float.IsInfinity(axis.x))
                    return;

                if (angle > 180f)
                    angle -= 360f;

                Vector3 angular = (0.9f * Mathf.Deg2Rad * angle * MoveForce) * axis.normalized;
                if (angular.sqrMagnitude > 0.05)
                    target.angularVelocity = angular;
                else
                    target.angularVelocity = Vector3.zero;

                target.linearVelocity = (floatingPosition - target.transform.position) * MoveForce;
            }
        }
    }
}