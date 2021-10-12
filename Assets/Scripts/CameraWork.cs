using System;
using UnityEngine;

namespace io.github.onikenx
{
    public class CameraWork : MonoBehaviour
    {
        #region Private Fields

        [Tooltip("The distance in the local x-z plane t the target")] [SerializeField]
        private float distance = 7.0f;

        [Tooltip("The height we want the camera to be above the target")] [SerializeField]
        private float height = 3.0f;

        [Tooltip("Allow the camera to be offseted verically from the target, for example giving more view of the sceneray and less ground.")] [SerializeField]
        private Vector3 centerOffset = Vector3.zero;

        [Tooltip("Set this as false if a component of a prefab being instanciated by Photon Network, and manully call OnStartFollowing() when and if  needed.")] [SerializeField]
        private bool followOnStart = false;

        [Tooltip("the Smothing for the camera to follow the target")] [SerializeField]
        private float smothSpeed = 0.125f;

        //cached transform of the target
        private Transform cameraTransform;

        //maintain a flag internally to reconnect if target is lost or camera is switched
        private bool isFollowing;

        //cCache for camera offset
        private Vector3 cameraOffset = Vector3.zero;

        #endregion

        #region Monobehaviour Callbacks

        private void Start()
        {
            //start following the target if wanted
            if (followOnStart)
                OnStartFollowing();
        }

        private void LateUpdate()
        {
            //the transform target may not destroy on level load
            //so we need to cover corner cases where the Main Camera is different everytime we load a new scene, and reconnect when that happens
            if (cameraTransform == null && isFollowing)
                OnStartFollowing();

            if (isFollowing)
                Follow();
        }
        
        #endregion

        #region Public Methods

        /// <summary>
        /// Raises the start following event.
        /// Use this when you don't know at the time of editing what to follow, typically instances managed by the photon network.
        /// </summary>
        public void OnStartFollowing()
        {
            if (Camera.main == null)
            {
                throw new Exception("Camera.main does not exist");
            }
            cameraTransform = Camera.main.transform;
            isFollowing = true;
            //we don't smooth anything, we go straight to the right camera shot
            Cut();
        }

        #endregion

        #region Private Methods

        /// <summary>
        /// follow the target smoothly
        /// </summary>
        private void Follow()
        {
            cameraOffset.z = -distance;
            cameraOffset.y = height;

            var position = this.transform.position;
            cameraTransform.position = Vector3.Lerp(cameraTransform.position, position + this.transform.TransformVector(cameraOffset), smothSpeed * Time.deltaTime);

            cameraTransform.LookAt(position + centerOffset);
        }

        private void Cut()
        {
            cameraOffset.z = -distance;
            cameraOffset.y = height;
            var position = this.transform.position;
            cameraTransform.position = position + this.transform.TransformVector(cameraOffset);

            cameraTransform.LookAt(position + centerOffset);
        }
        #endregion
    }
}