using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GD3D.CustomInput;

namespace GD3D.Player
{
    /// <summary>
    /// The class all gameplay scripts inherit from.
    /// </summary>
    public class GamemodeScript
    {
        [Header("Gravity")]
        [SerializeField] internal float gravity = 85;
        [SerializeField] internal float terminalVelocity = 28.4f;

        //-- Component references
        [HideInInspector] public PlayerGamemodeHandler GamemodeHandler;
        [HideInInspector] public PlayerMain Player;
        [HideInInspector] public Rigidbody Rigidbody;

        internal Transform _transform;
        internal GameObject _gameObject;

        /// <summary>
        /// Shortcut for setting and getting "rb.velocity.y"
        /// </summary>
        internal float YVelocity
        {
            get => GamemodeHandler.YVelocity;
            set => GamemodeHandler.YVelocity = value;
        }

        /// <summary>
        /// Shortcut for getting "p.dead"
        /// </summary>
        internal bool dead => Player._dead;

        /// <summary>
        /// Start is called before the first frame update
        /// </summary>
        public virtual void Start()
        {
            _transform = GamemodeHandler.transform;
            _gameObject = GamemodeHandler.gameObject;
        }

        /// <summary>
        /// OnEnable is called when the gamemode is switched to this gamemode
        /// </summary>
        public virtual void OnEnable()
        {

        }

        /// <summary>
        /// OnDisable is called when the gamemode is switched from this gamemode
        /// </summary>
        public virtual void OnDisable()
        {

        }

        /// <summary>
        /// Update is called once per frame
        /// </summary>
        public virtual void Update()
        {

        }

        /// <summary>
        /// Fixed Update is called once per physics frame
        /// </summary>
        public virtual void FixedUpdate()
        {
            // Gravity constant (do none if gravity is 0)
            if (gravity != 0)
            {
                Rigidbody.AddForce(Vector3.down * gravity);
            }

            // Clamp Y velocity between terminal velocity if it's not 0
            if (terminalVelocity != 0)
            {
                YVelocity = Mathf.Clamp(YVelocity, -terminalVelocity, terminalVelocity);
            }
        }

        /// <summary>
        /// OnClick is called when the player presses the main gameplay button. <para/>
        /// <paramref name="mode"/> determines whether the button was just pressed, held or just released.
        /// </summary>
        public virtual void OnClick(PressMode mode)
        {

        }

        public bool ButtonPress(PressMode mode = PressMode.hold)
        {
            return GamemodeHandler.ButtonPress(mode);
        }

        /// <summary>
        /// Fixed Update is called once per physics frame
        /// </summary>
        public virtual void OnDeath()
        {

        }

        /// <summary>
        /// Fixed Update is called once per physics frame
        /// </summary>
        public virtual void OnRespawn()
        {

        }
    }
}

// ############
// # Template #
// ############
/*
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Game.Input;

namespace Game.Player
{
    /// <summary>
    /// 
    /// </summary>
    [System.Serializable]
    public class INSERTNAME : GamemodeScript
    {
        /// <summary>
        /// OnEnable is called when the gamemode is switched to this gamemode
        /// </summary>
        public override void OnEnable()
        {
            base.OnEnable();
        }

        /// <summary>
        /// OnDisable is called when the gamemode is switched from this gamemode
        /// </summary>
        public override void OnDisable()
        {
            base.OnDisable();
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
        /// Fixed Update is called once per physics frame
        /// </summary>
        public override void OnClick(PressMode mode)
        {
            base.OnClick(mode);
        }
    }
}
*/
