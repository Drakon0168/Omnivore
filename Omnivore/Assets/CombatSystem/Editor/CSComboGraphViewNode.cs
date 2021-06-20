using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor.UIElements;
using UnityEditor.Experimental.GraphView;

namespace CombatSystem
{
    public class CSComboGraphViewNode : Node
    {
        private CSAttack data;
        private CSAttackElement attackElement;
        private bool entry;

        public CSAttack Data
        {
            get { return data; }
        }

        public CSComboGraphViewNode(CSAttack data, bool entry = false) : base()
        {
            this.data = data;
            this.entry = entry;
            title = data.Name;

            if (!entry)
            {
                VisualElement divider = new VisualElement();
                divider.name = "divider";
                divider.AddToClassList("horizontal");
                contentContainer.Q<VisualElement>("contents").Add(divider);

                attackElement = new CSAttackElement(this.data);
                contentContainer.Q<VisualElement>("contents").Add(attackElement);

                Label titleLabel = titleContainer.Q<Label>();
                TextField renameField = new TextField();
                renameField.style.display = DisplayStyle.None;

                titleLabel.AddManipulator(new Clickable(() =>
                {
                    titleLabel.style.display = DisplayStyle.None;
                    renameField.style.display = DisplayStyle.Flex;
                    renameField.Q<VisualElement>("unity-text-input").Focus();
                }));

                renameField.SetValueWithoutNotify(Data.Name);
                renameField.isDelayed = true;
                renameField.RegisterCallback<FocusOutEvent>(e =>
                {
                    titleLabel.style.display = DisplayStyle.Flex;
                    renameField.style.display = DisplayStyle.None;
                });
                renameField.RegisterValueChangedCallback(e =>
                {
                    Data.Name = e.newValue;
                    title = e.newValue;
                    titleLabel.text = title;

                    titleLabel.style.display = DisplayStyle.Flex;
                    renameField.style.display = DisplayStyle.None;
                });
                titleContainer.Add(renameField);
                renameField.SendToBack();
            }
        }

        public void Save(CSCombo asset)
        {
            data.GraphPosition = GetPosition().position;

            if (entry)
            {
                asset.Entry = data;
            }

            IEnumerator<VisualElement> children = outputContainer.Children().GetEnumerator();
            int chain = 0;

            while (children.MoveNext())
            {
                if(children.Current is Port)
                {
                    Port port = (Port)children.Current;

                    IEnumerator<Edge> edges = port.connections.GetEnumerator();

                    while (edges.MoveNext())
                    {
                        CSComboGraphViewNode node = (CSComboGraphViewNode)edges.Current.input.node;

                        asset.AddAttack(node.Data, Data, chain);
                        node.Save(asset);
                    }
                    chain++;
                }
            }
        }
    }
}