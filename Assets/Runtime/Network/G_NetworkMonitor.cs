/* -------------------------------------------------------------- *
 * Author:          Davina Armstrong (davina@playableworlds.com)  *
 * ---------------------------------------------------------------*/

using UnityEngine;
using UnityEngine.Profiling;

namespace Tayx.Graphy.Network
{
    public class G_NetworkMonitor : MonoBehaviour
    {
        #region Variables -> Private

        public int m_bytesReceived { get;  set; }  = 0;

        #endregion

        #region Properties -> Public

        public float BytesReceived => m_bytesReceived;

        #endregion

        #region Methods -> Unity Callbacks

        #endregion
    }
}