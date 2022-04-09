using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PathCreation;
using GD3D.CustomInput;

namespace GD3D.Player
{
    /// <summary>
    /// Handles the players constant movement and detects when the player is on ground or not. <para/>
    /// Also takes care of all movement on the second axis when 3D mode is enabled
    /// </summary>
    public class PlayerMovement : PlayerScript
    {
        //-- Speed constants (Blocks per second)
        // Numbers from: https://gdforum.freeforums.net/thread/55538/easy-speed-maths-numbers-speeds?page=1
        public const float SLOW_SPEED = 8.36820083682f; // Actual multiplier: 0.80648535564x
        public const float NORMAL_SPEED = 10.3761348898f; // Actual multiplier: 1x
        public const float DOUBLE_SPEED = 12.9032258065f; // Actual multiplier: 1.2435483871x
        public const float TRIPLE_SPEED = 15.5945419103f; // Actual multiplier: 1.5029239766x
        public const float QUADRUPLE_SPEED = 19.1846522782f; // Actual multiplier: 1.8489208633x

        [SerializeField] private PathCreator pathCreator;
        private VertexPath path => pathCreator.path;

        [Header("Stats")]
        [SerializeField] private GameSpeed currentSpeed = GameSpeed.normalSpeed;

        public static GameSpeed CurrentSpeed;
        public static float Speed;

        private float _travelAmount;
        private float _startTravelAmount;

        public float TravelAmount => _travelAmount;

        [Header("3D Mode (Moving on second axis)")]
        [SerializeField] private float speed3D = 1;

        [Space]
        [Range(0, 1)] [SerializeField] private float damp3DMoving = 0;
        [Range(0, 1)] [SerializeField] private float damp3DTurning = 0;
        [Range(0, 1)] [SerializeField] private float damp3DStopping = 0;

        [HideInInspector] public float OffsetVelocity;

        private Key leftKey;
        private Key rightKey;

        private Transform _cam;

        private bool _in3DMode;
        public bool In3DMode
        {
            get => _in3DMode;
            set
            {
                // Trigger effects if we are entering 3D mode
                if (value && !_in3DMode)
                {
                    enter3DModeParticles.Play();

                    arrowFlashAnim.SetTrigger("Flash");
                }
                // Trigger effects if we are exiting 3D mode
                else if (!value && _in3DMode)
                {
                    exit3DModeParticles.Play();
                }

                _in3DMode = value;
            }
        }

        [Header("Effects")]
        [SerializeField] private ParticleSystem enter3DModeParticles;
        [SerializeField] private ParticleSystem exit3DModeParticles;
        [SerializeField] private Animator arrowFlashAnim;

        private float _3DOffset;
        public float Current3DOffset => _3DOffset;

        private void Awake()
        {
            ChangeSpeed(currentSpeed);
        }

        public override void Start()
        {
            base.Start();

            // Set the transform
            _transform = transform;

            // Set the start travel amount
            _travelAmount = path.GetClosestDistanceAlongPath(transform.position);
            _startTravelAmount = _travelAmount;

            _cam = Helpers.Camera.transform;

            // Subscribe to the on respawn event
            player.OnRespawn += OnRespawn;

            leftKey = PlayerInput.GetKey("3D Move Left");
            rightKey = PlayerInput.GetKey("3D Move Right");
        }

        public override void FixedUpdate()
        {
            base.FixedUpdate();

            Extra3DModeMovement();

            // Move the player
            _travelAmount += Time.fixedDeltaTime * Speed;

            // Calculate target position
            Vector3 targetPos = path.GetPointAtDistance(_travelAmount, EndOfPathInstruction.Stop);
            Vector3 direction = path.GetNormalAtDistance(_travelAmount, EndOfPathInstruction.Stop);

            targetPos += direction * _3DOffset;

            // Ignore Y
            targetPos.y = _transform.position.y;

            _transform.position = targetPos;

            // Rotate the correct way
            Vector3 newRot = path.GetRotationAtDistance(_travelAmount, EndOfPathInstruction.Stop).eulerAngles;

            newRot.x = transform.rotation.eulerAngles.x;
            newRot.z = transform.rotation.eulerAngles.z;

            transform.rotation = Quaternion.Euler(newRot);
        }

        private void Extra3DModeMovement()
        {
            // IMPLEMENT EASING HERE

            // Guard clause
            if (!_in3DMode) //|| _current3DOffsetTween != null)
            {
                if (OffsetVelocity != 0)
                {
                    OffsetVelocity = 0;
                }

                return;
            }

            // Input
            float input = Key.GetAxis(leftKey, rightKey);

            // Transform the input to be relative to the camera
            Vector3 test1 = _transform.TransformDirection(new Vector3(0, 0, input));
            Vector3 test2 = _cam.TransformDirection(new Vector3(0, 0, input));

            print(test1 + " | " + test2);

            // By default, damping is moving
            float damping = damp3DMoving;

            // Turning
            if (Mathf.Sign(input) != Mathf.Sign(OffsetVelocity))
            {
                damping = damp3DTurning;
            }
            // Stopping
            else if (input == 0)
            {
                damping = damp3DStopping;
            }

            // Apply velocity
            OffsetVelocity += input * (speed3D / 10);
            OffsetVelocity *= Mathf.Pow(damping, Time.fixedDeltaTime * 10);

            // Change offset
            _3DOffset += OffsetVelocity;
            _3DOffset = Mathf.Clamp(_3DOffset, -4.5f, 4.5f);
        }

        /// <summary>
        /// Returns what the players speed is going to be at the given <paramref name="distance"/> <para/>
        /// Note: This currently just returns the players speed as we have no speed portals in the game
        /// </summary>
        public float GetSpeedAtDistance(float distance)
        {
            switch (currentSpeed)
            {
                case GameSpeed.slowSpeed:
                    return SLOW_SPEED;

                case GameSpeed.normalSpeed:
                    return NORMAL_SPEED;

                case GameSpeed.doubleSpeed:
                    return DOUBLE_SPEED;

                case GameSpeed.tripleSpeed:
                    return TRIPLE_SPEED;

                case GameSpeed.quadrupleSpeed:
                    return QUADRUPLE_SPEED;
            }

            // Return the current speed by default
            return Speed;
        }

        /// <summary>
        /// Changes the players speed to match the <paramref name="newSpeed"/>
        /// </summary>
        public void ChangeSpeed(GameSpeed newSpeed)
        {
            currentSpeed = newSpeed;
            CurrentSpeed = newSpeed;

            // Set moveSpeed based on the current speed
            switch (newSpeed)
            {
                case GameSpeed.slowSpeed:
                    Speed = SLOW_SPEED;
                    break;

                case GameSpeed.normalSpeed:
                    Speed = NORMAL_SPEED;
                    break;

                case GameSpeed.doubleSpeed:
                    Speed = DOUBLE_SPEED;
                    break;

                case GameSpeed.tripleSpeed:
                    Speed = TRIPLE_SPEED;
                    break;

                case GameSpeed.quadrupleSpeed:
                    Speed = QUADRUPLE_SPEED;
                    break;

                default:
                    Speed = 0;
                    break;
            }
        }

        /// <summary>
        /// Cancels the current 3D offset tween if there is one currently active
        /// </summary>
        public void Cancel3DOffsetTween()
        {
            /*
            // Cancel the current 3D offset tween if it exists
            if (_current3DOffsetTween != null)
            {
                _current3DOffsetTween.cancel();
                _current3DOffsetTween = null;
            }
            */
        }

        /// <summary>
        /// Will tween the 3D offset to the given <paramref name="target"/> value over the given <paramref name="time"/> with the given <paramref name="easeType"/>.
        /// </summary>
        public void Tween3DOffset(float target, float time = 1, EasingType easeType = EasingType.sineInOut)
        {
            /*
            Cancel3DOffsetTween();
            
            // Tween to the target value
            _current3DOffsetTween = LeanTween.value(_3DOffset, target, time)
                .SetGDEase(easeType)
                .setOnUpdate((value) =>
                {
                    _3DOffset = Mathf.Clamp(value, -4.5f, 4.5f);
                }
            ).setOnComplete(() => _current3DOffsetTween = null);
            */
        }
            
        /// <summary>
        /// Is called when the player respawns
        /// </summary>
        private void OnRespawn()
        {
            // Reset the travel amount and position
            _travelAmount = _startTravelAmount;

            _transform.position = player.startPos;

            // Reset rigidbody components aswell
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }

    public enum GameSpeed
    {
        none = -2,
        slowSpeed = -1,
        normalSpeed = 0,
        doubleSpeed = 1,
        tripleSpeed = 2,
        quadrupleSpeed = 4,
    }
}