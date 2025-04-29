using UnityEngine;

namespace Heron
{
    public class Player_Human : Player_Base
    {

        #region Protected Methods

        protected override InputStruct GetInputs()
        {
            InputStruct ret = new InputStruct
            {
                Horizontal = Input.GetAxis( "Horizontal" ),
                Vertical   = Input.GetAxis( "Vertical" ),
            };
            return ret;
        }

        #endregion

        protected override float OutlineWidth => 20f;
    }
}