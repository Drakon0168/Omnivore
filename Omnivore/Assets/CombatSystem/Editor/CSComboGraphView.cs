using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;
using UnityEditor.Experimental.GraphView;

namespace CombatSystem
{
    public class CSComboGraphView : GraphView
    {
        private CSCombo asset;
        private CSComboGraphViewNode entry;

        public void Setup(CSCombo asset)
        {
            this.asset = asset;

            styleSheets.Add(Resources.Load<StyleSheet>("CSStyleSheet"));

            SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);

            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(new RectangleSelector());
            this.AddManipulator(new ContentDragger());

            GridBackground grid = new GridBackground();
            grid.StretchToParentSize();
            Insert(0, grid);

            Load();
        }

        /// <summary>
        /// Loads the graph data from the CSCombo
        /// </summary>
        private void Load()
        {
            Port port;
            entry = GenerateNode(asset.Entry, out port, true);
        }

        /// <summary>
        /// Saves the graph layout to the CSCombo
        /// </summary>
        public void Save()
        {
            asset.RemoveAttack(asset.Entry);
            entry.Save(asset);

            EditorUtility.SetDirty(asset);
            AssetDatabase.SaveAssets();
        }

        #region Node Management

        /// <summary>
        /// Creates a new node and adds it to the graph
        /// </summary>
        /// <param name="name">The name to give the new node</param>
        public void AddNode(string name)
        {
            CSAttack newData = new CSAttack(name);
            asset.AddAttack(newData, null, 0);

            Port port = null;
            GenerateNode(newData, out port);
        }

        /// <summary>
        /// Generates a node and adds it to the graph
        /// </summary>
        /// <param name="data">The data to use when creating the new node</param>
        /// <returns>The node that was created</returns>
        private CSComboGraphViewNode GenerateNode(CSAttack data, out Port inputPort, bool entry = false)
        {
            CSComboGraphViewNode newNode = new CSComboGraphViewNode(data, entry);
            newNode.SetPosition(new Rect(data.GraphPosition, new Vector2(200, 150)));

            if (entry)
            {
                inputPort = null;
            }
            else
            {
                inputPort = GeneratePort(newNode, Direction.Input);
            }

            for(int i = 0; i < asset.Chains.Length; i++)
            {
                Port outputPort = GeneratePort(newNode, Direction.Output);
                outputPort.portName = asset.Chains[i];

                if(data.Chains[i] != null)
                {
                    Port newInputPort = null;
                    CSComboGraphViewNode currentChain = GenerateNode(data.Chains[i], out newInputPort);

                    AddElement(outputPort.ConnectTo(newInputPort));
                }
            }

            newNode.RefreshExpandedState();
            newNode.RefreshPorts();
            AddElement(newNode);
            return newNode;
        }

        #endregion

        #region Port Management

        /// <summary>
        /// Generates a port and adds it to the specified node
        /// </summary>
        /// <param name="node">The node to generate the port for</param>
        /// <param name="direction">Wether the port is an input or an output</param>
        private Port GeneratePort(CSComboGraphViewNode node, Direction direction)
        {
            Port newPort = node.InstantiatePort(Orientation.Horizontal, direction, Port.Capacity.Single, typeof(CSAttack));

            switch (direction)
            {
                case Direction.Input:
                    newPort.portName = "Input";
                    node.inputContainer.Add(newPort);
                    break;
                case Direction.Output:
                    node.outputContainer.Add(newPort);
                    break;
            }

            return newPort;
        }

        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            List<Port> compatiblePorts = new List<Port>();

            ports.ForEach(port =>
            {
                if(port.node != startPort.node && port.direction != startPort.direction)
                {
                    compatiblePorts.Add(port);
                }
            });

            return compatiblePorts;
        }

        #endregion
    }
}