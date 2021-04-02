/* -------------------------------------------------------------- *
 * Author:          Davina Armstrong (davina@playableworlds.com)  *
 * ---------------------------------------------------------------*/

using Tayx.Graphy.Graph;
using UnityEngine;
using UnityEngine.UI;

namespace Tayx.Graphy.Network
{
    public class G_NetworkGraph: G_Graph
    {
        #region Variables -> Serialized Private

        [SerializeField] private Image m_imageGraph = null;

        [SerializeField] private Shader ShaderFull = null;
        [SerializeField] private Shader ShaderLight = null;

        // This keeps track of whether Init() has run or not
        [SerializeField] private bool m_isInitialized = false;

        #endregion

        #region Variables -> Private

        private GraphyManager m_graphyManager = null;

        private G_NetworkMonitor m_networkMonitor = null;

        private int m_resolution = 150;

        private G_GraphShader m_shaderGraph = null;

        private int[] m_bytesInArray;

        private int m_highestBytesIn;

        #endregion

        #region Methods -> Unity Callbacks

        private void Update()
        {
            UpdateGraph();
        }

        #endregion

        #region Methods -> Public

        public void UpdateParameters()
        {
            if (m_shaderGraph == null)
            {
                // TODO: While Graphy is disabled (e.g. by default via Ctrl+H) and while in Editor after a Hot-Swap,
                // the OnApplicationFocus calls this while m_shaderGraph == null, throwing a NullReferenceException
                return;
            }
            m_shaderGraph.ArrayMaxSize = G_GraphShader.ArrayMaxSizeFull;
            m_shaderGraph.Image.material = new Material(ShaderFull);

            m_shaderGraph.InitializeShader();

            CreatePoints();
        }

        #endregion

        #region Methods -> Protected Override

        protected override void UpdateGraph()
        {
            // Since we no longer initialize by default OnEnable(), 
            // we need to check here, and Init() if needed
            if (!m_isInitialized)
            {
                Init();
            }

            int bytesIn = m_networkMonitor.m_bytesReceived;

            int currentMaxBytesIn = 0;

            for (int i = 0; i <= m_resolution - 1; i++)
            {
                if (i >= m_resolution - 1) // == m_resolution - 1
                {
                    m_bytesInArray[i] = bytesIn;
                }
                else
                {
                    m_bytesInArray[i] = m_bytesInArray[i + 1];
                }

                // Store the highest bytesIn to use as the highest point in the graph

                if (currentMaxBytesIn < m_bytesInArray[i])
                {
                    currentMaxBytesIn = m_bytesInArray[i];
                }

            }

            m_highestBytesIn = m_highestBytesIn < 1 || m_highestBytesIn <= currentMaxBytesIn ? currentMaxBytesIn : m_highestBytesIn - 1;

            if (m_shaderGraph.Array == null)
            {
                m_bytesInArray = new int[m_resolution];
                m_shaderGraph.Array = new float[m_resolution];
            }

            for (int i = 0; i <= m_resolution - 1; i++)
            {
                m_shaderGraph.Array[i] = m_bytesInArray[i] / (float)m_highestBytesIn;
            }

            // Update the material values

            m_shaderGraph.UpdatePoints();
        }

        protected override void CreatePoints()
        {
            if (m_shaderGraph.Array == null || m_bytesInArray.Length != m_resolution)
            {
                m_bytesInArray = new int[m_resolution];
                m_shaderGraph.Array = new float[m_resolution];
            }

            for (int i = 0; i < m_resolution; i++)
            {
                m_shaderGraph.Array[i] = 0;
            }

            m_shaderGraph.BytesInColor = m_graphyManager.BytesInColor;

            m_shaderGraph.UpdateColors();

            m_shaderGraph.UpdateArray();
        }

        #endregion

        #region Methods -> Private

        private void Init()
        {
            m_graphyManager = transform.root.GetComponentInChildren<GraphyManager>();

            m_networkMonitor = GetComponent<G_NetworkMonitor>();

            m_shaderGraph = new G_GraphShader
            {
                Image = m_imageGraph
            };

            UpdateParameters();

            m_isInitialized = true;
        }

        #endregion
    }
}