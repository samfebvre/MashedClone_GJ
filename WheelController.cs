using UnityEngine;

public class WheelController : MonoBehaviour
{

    #region Serialized

    public Transform WheelModel;

    [HideInInspector] public WheelCollider WheelCollider;

    public bool Steerable;
    public bool Motorized;

    #endregion

    #region Public Methods

    public void Init()
    {
        WheelCollider = GetComponent<WheelCollider>();
    }

    #endregion

    #region Unity Functions

    private void Update()
    {
        WheelCollider.GetWorldPose( out m_position, out m_rotation );
        WheelModel.transform.position = m_position;
        WheelModel.transform.rotation = m_rotation;
    }

    #endregion

    #region Private Fields

    private Vector3    m_position;
    private Quaternion m_rotation;

    #endregion

}