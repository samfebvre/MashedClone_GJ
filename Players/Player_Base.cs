using System;
using System.Text;
using Heron.Utils;
using UnityEngine;
using static Heron.Player_Base.PlayerRaceState;
using Object = UnityEngine.Object;

namespace Heron
{
    public abstract class Player_Base
    {

        #region Statics and Constants

        private const float TIME_TO_KILL_IMMOBILE_PLAYERS                = 4;
        private const float VELOCITY_THRESHOLD_TO_BE_CONSIDERED_IMMOBILE = 1.0f;

        #endregion

        #region Public Enums

        public enum PlayerRaceState
        {
            None,
            WaitingToStartRound,
            Racing,
            Dead,
            WonRound,
            Finished,
        }

        #endregion

        #region Public Fields

        public StringBuilder DebugTextStringBuilder = new StringBuilder();

        public int Score;

        #endregion

        #region Public Properties

        public bool CanUpdateWheelForces => RaceState is Racing or WonRound or Finished;

        public Color PlayerColor { get; private set; }

        public Player_MonoBehaviour PlayerMonoBehaviour { get; private set; }

        public string PlayerName { get; private set; }

        public PlayerRaceState RaceState { get; set; }

        public Transform Transform     => PlayerMonoBehaviour.transform;
        public Vector3   WorldPosition => PlayerMonoBehaviour.transform.position;

        #endregion

        #region Public Methods

        public void DestroyVehicleObject()
        {
            if ( PlayerMonoBehaviour == null )
            {
                return;
            }

            Object.Destroy( PlayerMonoBehaviour.gameObject );
        }
        
        protected virtual float OutlineWidth => 0;

        public virtual void Init_MonoBehaviour( Player_MonoBehaviour playerMonoBehaviour, Material vehicleMaterial )
        {
            PlayerMonoBehaviour = playerMonoBehaviour;
            PlayerMonoBehaviour.Init( this, vehicleMaterial );
            PlayerMonoBehaviour.gameObject.name                     = $"Obj: {PlayerName}";
            PlayerMonoBehaviour.GetComponent<Outline>().OutlineWidth = OutlineWidth;
        }

        public void Init_Player( string playerName, Color playerColor )
        {
            GameManager       = GameManager.Instance;
            RaceManager       = GameManager.RaceManager;
            CheckpointManager = GameManager.CheckPointManager;

            PlayerColor = playerColor;
            PlayerName  = playerName;
        }

        public void KillPlayer()
        {
            RaceState = Dead;
            RaceManager.OnPlayerDied();
        }

        #if UNITY_EDITOR
        public void OnDrawGizmos()
        {
            if ( !GameManager.PlayerDebugSettings.Debug_ShowLabels )
            {
                return;
            }

            string debugTextTrimmed = CollectAllDebugText();
            GizmoUtils.DrawALabelBoxWithInfo( PlayerMonoBehaviour.transform.position, debugTextTrimmed, 20, FontStyle.Bold, PlayerColor );
        }
        #endif

        public void OnEnteredCheckpointVolume( GameObject checkpoint )
        {
            if ( RaceState != Dead )
            {
                CheckpointManager.OnPlayerEnteredCheckpoint( this, checkpoint );
            }
        }

        public void OnEnteredKillVolume()
        {
            if ( RaceState != Dead )
            {
                KillPlayer();
            }
        }

        public void OnExitedCheckpointVolume( GameObject checkpoint )
        {
            if ( RaceState != Dead )
            {
                CheckpointManager.OnPlayerExitedCheckpoint( this, checkpoint );
            }
        }

        public void OnFixedUpdate()
        {
            PlayerMonoBehaviour.SetInputs( GetInputs() );
            PlayerMonoBehaviour.OnFixedUpdate();
            DetectPlayerImmobileAndKillIfNecessary();
        }

        public virtual void ResetPlayer()
        {
            RaceState = None;
        }

        #endregion

        #region Protected Fields

        protected CheckpointManager CheckpointManager;

        protected GameManager GameManager;

        protected RaceManager RaceManager;

        #endregion

        #region Protected Methods

        protected abstract InputStruct GetInputs();

        protected virtual void PopulateStringBuilderWithDebugText( ref StringBuilder stringBuilder )
        {
            stringBuilder.AppendLine( PlayerName );
            stringBuilder.AppendLine( $"Race State: {RaceState}" );
            stringBuilder.AppendLine( $"Time Since Got Stuck: {TimeSinceGotStuck}" );

            // populate with relevant checkpoint manager info
            stringBuilder.AppendLine( $"Checkpoint: {CheckpointManager.GetCurrentCheckpointIndexForPlayer( this )}" );
            stringBuilder.AppendLine( $"Lap: {CheckpointManager.GetCurrentLapForPlayer( this )}" );
            stringBuilder.AppendLine( $"Position: {CheckpointManager.GetCurrentPositionForPlayer( this )}" );
        }

        #endregion

        #region Private Properties

        private float TimeSinceGotStuck { get; set; }

        #endregion

        #region Private Methods

        private string CollectAllDebugText()
        {
            // First clear the debug text
            DebugTextStringBuilder.Clear();
            // Append all the stuff we want to debug from this class
            PopulateStringBuilderWithDebugText( ref DebugTextStringBuilder );
            // Trim any trailing newlines
            string debugTextTrimmed = DebugTextStringBuilder.ToString().TrimEnd( Environment.NewLine.ToCharArray() );
            return debugTextTrimmed;
        }

        private void DetectPlayerImmobileAndKillIfNecessary()
        {
            if ( RaceState is not (Racing or WonRound) )
            {
                TimeSinceGotStuck = 0;
                return;
            }

            if ( PlayerMonoBehaviour.Rigidbody.velocity.magnitude < VELOCITY_THRESHOLD_TO_BE_CONSIDERED_IMMOBILE )
            {
                TimeSinceGotStuck += Time.fixedDeltaTime;
            }
            else
            {
                TimeSinceGotStuck = 0;
            }

            if ( TimeSinceGotStuck > TIME_TO_KILL_IMMOBILE_PLAYERS )
            {
                KillPlayer();
            }
        }

        #endregion

    }

    [Serializable]
    public struct PlayerDebugSettings
    {

        #region Serialized

        public bool Debug_ShowLabels;

        #endregion

    }
}