using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SplineAI
{
    [DefaultExecutionOrder(100)]
    public class SplineManager : MonoBehaviour
    {
        public float NodeCombineDist = 5.0f;
        public float nodeEditorSize = 0.5f;

        public struct NodeIndexInSpline
        {
            // array of the stacked nodes and their spline and index in the spline
            public UISpline spline;
            public int splineIndex;
        }

        public struct Node
        {
            // array of the stacked nodes and their spline and index in the spline
            public List<NodeIndexInSpline> nodesIndexInSpline;

            // info on position, connections to other nodes appropriate to this 
            public Vector3 position;
            public List<int> connections;
        }

        [HideInInspector]
        public List<Node> nodes = new List<Node>();

        // Start is called before the first frame update
        void Start()
        {
            GameObject parent = this.transform.parent.gameObject;
            UISpline[] UISplines = parent.GetComponentsInChildren<UISpline>();

            foreach (UISpline uISpline in UISplines)
                AddSpline(uISpline);
        }

        private Vector3 offsetPos(Vector3 p, float aa)
        {
            float x = p.x;
            float y = p.y;
            float z = p.z;

            // radius
            float r = Mathf.Sqrt((x * x) + (z * z));

            // angle
            float a = Mathf.Atan2(z, x) - (aa * (Mathf.PI / 180));

            x = r * Mathf.Cos(a);
            z = r * Mathf.Sin(a);

            return new Vector3(x, y, z);
        }

        void AddSpline(UISpline uISpline)
        {
            float a = uISpline.gameObject.transform.eulerAngles.y;
            int prevIndex = -1;

            int splineIndex = 0;
            foreach (Vector3 point in uISpline.points)
            {
                // attempt to find a duplicate
                int index = nodes.Count;
                for (int i = 0; i < nodes.Count; i++)
                {
                    float dist = Vector3.Distance(offsetPos(point, a) + uISpline.transform.parent.position, nodes[i].position);
                    if (dist < NodeCombineDist)
                        index = i;
                }

                // add a new node if this is unique
                if (index == nodes.Count)
                {
                    Node node = new Node();
                    node.nodesIndexInSpline = new List<NodeIndexInSpline>();
                    node.position = offsetPos(point, a) + uISpline.transform.parent.position;
                    node.connections = new List<int>();
                    nodes.Add(node);
                }

                // connect this node here to the previous index if its valid
                if (prevIndex != -1)
                {
                    nodes[index].connections.Add(prevIndex); // make as forwards connection
                    nodes[prevIndex].connections.Add(index);
                }

                prevIndex = index;

                // make backwards compatible to the splines of the node and it's index there
                NodeIndexInSpline n = new NodeIndexInSpline();
                n.spline = uISpline;
                n.splineIndex = splineIndex;
                nodes[index].nodesIndexInSpline.Add(n);

                // go to next index
                splineIndex++;
            }
        }

        public int GetClosestNode(Vector3 pos)
        {
            int closestIndex = 0;
            float curDistance = Vector3.Distance(pos, nodes[0].position);

            for (int i = 1; i < nodes.Count; i++)
            {
                float thisDistance = Vector3.Distance(pos, nodes[i].position);
                if (thisDistance < curDistance)
                {
                    closestIndex = i;
                    curDistance = thisDistance;
                }
            }

            return closestIndex;
        }
    }
}