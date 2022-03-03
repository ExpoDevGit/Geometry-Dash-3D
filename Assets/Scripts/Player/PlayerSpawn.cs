using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace GD3D.Player
{
    /// <summary>
    /// Controls the player spawning
    /// </summary>
    public class PlayerSpawn : PlayerScript
    {
        [SerializeField] private GameObject respawnRing;

        [SerializeField] private float respawnTime;

        [SerializeField] private TMP_Text attemptText;
        private int _currentAttemp = 1;

        /// <summary>
        /// Start is called before the first frame update
        /// </summary>
        public override void Start()
        {
            base.Start();

            // Subscribe to the OnDeath event
            player.OnDeath += OnDeath;
        }

        /// <summary>
        /// Update is called once per frame
        /// </summary>
        public override void Update()
        {
            base.Update();
        }

        /// <summary>
        /// Fixed Update is called once per physics frame
        /// </summary>
        public override void FixedUpdate()
        {
            base.FixedUpdate();
        }

        /// <summary>
        /// Is called when the player dies
        /// </summary>
        private void OnDeath()
        {
            // Disable the mesh
            player.mesh.ToggleCurrentMesh(false);

            // Stop the currently active respawn coroutine
            if (currentRespawnCoroutine != null)
            {
                StopCoroutine(currentRespawnCoroutine);
            }

            // Start the respawn coroutine
            currentRespawnCoroutine = StartCoroutine(Respawn());
        }

        private Coroutine currentRespawnCoroutine;
        /// <summary>
        /// Makes the player flash on/off and spawn respawn rings
        /// </summary>
        public IEnumerator Respawn()
        {
            // Wait 1 second
            yield return new WaitForSeconds(1f);

            _currentAttemp++;
            attemptText.text = "Attempt " + _currentAttemp;

            // Invoke respawn event
            player.InvokeRespawnEvent();

            // Make the player flash on/off and spawn respawn rings every time the player is turned on
            // Do this 3 times total over the course of 0.6 seconds
            SpawnRespawnRing();
            ToggleMesh(true);

            for (int i = 0; i < 3; i++)
            {
                yield return new WaitForSeconds(0.05f);

                ToggleMesh(false);

                yield return new WaitForSeconds(0.05f);

                SpawnRespawnRing();
                ToggleMesh(true);
            }
        }

        /// <summary>
        /// Just a shortcut for <see cref="PlayerMesh.ToggleCurrentMesh(bool)"/>
        /// </summary>
        private void ToggleMesh(bool enable)
        {
            player.mesh.ToggleCurrentMesh(enable);
        }

        /// <summary>
        /// Spawns a respawn ring with the right color
        /// </summary>
        private void SpawnRespawnRing()
        {
            // Create the ring
            GameObject obj = Instantiate(respawnRing, transform.position, Quaternion.identity, transform);
            obj.transform.localPosition = Vector3.zero;

            // Change the line renderers color
            LineRenderer lr = obj.GetComponent<LineRenderer>();
            lr.startColor = PlayerColor1;
            lr.endColor = PlayerColor1;
        }
    }
}
