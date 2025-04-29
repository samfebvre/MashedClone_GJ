using Heron;
using UnityEngine;

public class VehicleController : MonoBehaviour
{

    #region Serialized

    public float MotorTorque             = 2000;
    public float BrakeTorque             = 2000;
    public float MaxSpeed                = 20;
    public float SteeringRange           = 30;
    public float SteeringRangeAtMaxSpeed = 10;
    public float CentreOfGravityOffset   = -1f;

    #endregion

    #region Public Properties

    public BoxCollider BodyCollider
    {
        get
        {
            if ( m_bodyCollider == null )
            {
                m_bodyCollider = BodyTransform.GetComponent<BoxCollider>();
            }

            return m_bodyCollider;
        }
    }

    public Transform BodyTransform
    {
        get
        {
            if ( m_bodyTransform == null )
            {
                m_bodyTransform = transform.Find( "Body" );
            }

            return m_bodyTransform;
        }
    }

    #endregion

    #region Public Methods

    public Vector3 GetVehicleColliderCentre() =>
        BodyCollider.center;

    public Vector3 GetVehicleColliderSize() => BodyCollider.size;

    public void Init( Player_MonoBehaviour playerMonoBehaviour )
    {
        PlayerMonoBehaviour = playerMonoBehaviour;
        RaceManager         = GameManager.Instance.RaceManager;

        m_rigidBody = GetComponent<Rigidbody>();

        m_rigidBody.centerOfMass += Vector3.up * CentreOfGravityOffset;

        m_wheels = GetComponentsInChildren<WheelController>();
        foreach ( WheelController wheelController in m_wheels )
        {
            wheelController.Init();
        }
    }

    public void OnFixedUpdate()
    {
        if ( PlayerMonoBehaviour.PlayerBase.CanUpdateWheelForces )
        {
            UpdateWheelForces();
        }
    }

    public void ResetWheelForces()
    {
        foreach ( WheelController wheel in m_wheels )
        {
            if ( wheel.Steerable )
            {
                wheel.WheelCollider.steerAngle = 0;
            }

            wheel.WheelCollider.motorTorque = 0;
        }
    }

    public void SetInputs( InputStruct inputs )
    {
        m_verticalInput   = inputs.Vertical;
        m_horizontalInput = inputs.Horizontal;
    }

    #endregion

    #region Private Fields

    private BoxCollider       m_bodyCollider;
    private Transform         m_bodyTransform;
    private float             m_horizontalInput;
    private Rigidbody         m_rigidBody;
    private float             m_verticalInput;
    private WheelController[] m_wheels;

    #endregion

    #region Private Properties

    private Player_MonoBehaviour PlayerMonoBehaviour { get; set; }
    private RaceManager          RaceManager         { get; set; }

    #endregion

    #region Private Methods

    private void UpdateWheelForces()
    {
        float forwardSpeed       = Vector3.Dot( transform.forward, m_rigidBody.velocity );
        float speedFactor        = Mathf.InverseLerp( 0, MaxSpeed, forwardSpeed );
        float currentMotorTorque = Mathf.Lerp( MotorTorque,   0,                       speedFactor );
        float currentSteerRange  = Mathf.Lerp( SteeringRange, SteeringRangeAtMaxSpeed, speedFactor );
        bool  isAccelerating     = Mathf.Sign( m_verticalInput ).Equals( Mathf.Sign( forwardSpeed ) );

        foreach ( WheelController wheel in m_wheels )
        {
            if ( wheel.Steerable )
            {
                wheel.WheelCollider.steerAngle = m_horizontalInput * currentSteerRange;
            }

            if ( isAccelerating )
            {
                if ( wheel.Motorized )
                {
                    wheel.WheelCollider.motorTorque = m_verticalInput * currentMotorTorque;
                }

                wheel.WheelCollider.brakeTorque = 0;
            }
            else
            {
                wheel.WheelCollider.brakeTorque = Mathf.Abs( m_verticalInput ) * BrakeTorque;
                wheel.WheelCollider.motorTorque = 0;
            }
        }
    }

    #endregion

}