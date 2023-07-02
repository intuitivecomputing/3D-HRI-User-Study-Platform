using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using SplineMesh;

public class RobotSpline : MonoBehaviour
{
    // public int length;
    // Start is called before the first frame update
    public GameObject spline_obj;

    [SerializeField]
    public List<GameObject> joints = new List<GameObject>();

    // private List<SplineNode> spline_nodes;
    // private Transform spline_transform;
    private Spline spline;
    private bool toUpdate = false;
    void Start()
    {
        // spline_transform = spline_obj.transform;
        spline = spline_obj.GetComponent<Spline>();


        // spline_nodes = spline_obj.GetComponent<Spline>().nodes;
        // spline_nodes = new List<SplineNode>(length);

        toUpdate = true;
    }

    // Update is called once per frame
    void Update()
    {
        // Debug.Log("---------------------");
        // Transform current_transform = gameObject.transform;
        // foreach (SplineNode node in spline_nodes)
        // {
        //     current_transform = current_transform.parent;
        //     Debug.Log(spline_transform.InverseTransformPoint(current_transform.position));
        //     node.Position = spline_transform.InverseTransformPoint(current_transform.position);
        //     // node.Direction =
        // }

        if (toUpdate)
        {
            toUpdate = false;
            UpdateSpline();
        }
        UpdateNodes();

    }

    private void UpdateNodes()
    {
        int i = 0;
        foreach (GameObject joint in joints)
        {
            var node = spline.nodes[i++];
            if (Vector3.Distance(node.Position, transform.InverseTransformPoint(joint.transform.position)) > 0.001f)
            {
                node.Position = transform.InverseTransformPoint(joint.transform.position);
                // node.Direction = joint.transform.eulerAngles;
                node.Up = joint.transform.up;
            }
        }
    }

    private void UpdateSpline()
    {
        foreach (var penisNode in joints.ToList())
        {
            if (penisNode == null) joints.Remove(penisNode);
        }
        int nodeCount = joints.Count;
        // adjust the number of nodes in the spline.
        while (spline.nodes.Count < nodeCount)
        {
            spline.AddNode(new SplineNode(Vector3.zero, Vector3.zero));
        }
        while (spline.nodes.Count > nodeCount && spline.nodes.Count > 2)
        {
            spline.RemoveNode(spline.nodes.Last());
        }
    }
}
