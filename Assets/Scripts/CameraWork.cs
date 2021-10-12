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
            UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
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

        /// <summary>See CalledOnLevelWasLoaded. Outdated in Unity 5.4.</summary>
        void OnLevelWasLoaded(int level)
        {
            this.CalledOnLevelWasLoaded(level);
        }


        void CalledOnLevelWasLoaded(int level)
        {
            // check if we are outside the Arena and if it's the case, spawn around the center of the arena in a safe zone
            if (!Physics.Raycast(transform.position, -Vector3.up, 5f))
            {
                transform.position = new Vector3(0f, 5f, 0f);
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Raises the start following event.
        /// Use this when you don't know at the time of editing what to follow, typically instances managed by the photon network.
        /// </summary>
        public void OnStartFollowing()
        {
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
        void Follow()
        {
            cameraOffset.z = -distance;
            cameraOffset.y = height;

            cameraTransform.position = Vector3.Lerp(cameraTransform.position, this.transform.position + this.transform.TransformVector(cameraOffset), smothSpeed * Time.deltaTime);

            cameraTransform.LookAt(this.transform.position + centerOffset);
        }

        void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, UnityEngine.SceneManagement.LoadSceneMode loadingMode)
        {
            this.CalledOnLevelWasLoaded(scene.buildIndex);
        }

        void Cut()
        {
            cameraOffset.z = -distance;
            cameraOffset.y = height;

            cameraTransform.position = this.transform.position + this.transform.TransformVector(cameraOffset);

            cameraTransform.LookAt(this.transform.position + centerOffset);
        }

        #endregion
    }
}