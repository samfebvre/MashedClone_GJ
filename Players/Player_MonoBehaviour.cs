﻿using UnityEngine;

namespace Heron
{
    public class Player_MonoBehaviour : MonoBehaviour
    {

        #region Statics and Constants

        private static readonly int DESIRED_COLOR = Shader.PropertyToID( "_DesiredColor" );

        #endregion

        #region Public Properties

        public float Height { get; private set; }

        public Player_Base PlayerBase { get; private set; }
        public Rigidbody   Rigidbody  { get; private set; }

        #endregion

        #region Public Methods

        public void Init( Player_Base player_Base, Material vehicleMaterial )
        {
            PlayerBase = player_Base;

            m_vehicleController = GetComponent<VehicleController>();
            m_vehicleController.Init( this );

            Rigidbody = GetComponent<Rigidbody>();

            Transform bodyTransform = transform.Find( "Body" );
            m_bodyMeshRenderer          = bodyTransform.GetComponent<MeshRenderer>();
            m_bodyMeshRenderer.material = vehicleMaterial;
            m_bodyMeshRenderer.material.SetColor( DESIRED_COLOR, PlayerBase.PlayerColor );

            Height = bodyTransform.GetComponent<BoxCollider>().size.y;
        }

        public void OnFixedUpdate()
        {
            m_vehicleController.OnFixedUpdate();
        }

        public void ResetWheelForces()
        {
            m_vehicleController.ResetWheelForces();
        }

        public void SetInputs( InputStruct inputs )
        {
            m_vehicleController.SetInputs( inputs );
        }

        #endregion

        #region Private Fields

        private MeshRenderer m_bodyMeshRenderer;

        private Rect m_labelRect;

        private VehicleController m_vehicleController;

        #endregion

    }
}