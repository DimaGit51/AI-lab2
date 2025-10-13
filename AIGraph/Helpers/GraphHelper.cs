using AIGraph.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIGraph.Helpers
{
    public static class GraphHelper
    {
        public static Node GetNodeAt(List<Node> nodes, Point p, int radius)
        {
            for (int i = 0; i < nodes.Count; i++)
            {
                Node node = nodes[i];
                double dx = p.X - node.Position.X;
                double dy = p.Y - node.Position.Y;
                double distance = Math.Sqrt(dx * dx + dy * dy);
                if (distance <= radius)
                    return node;
            }
            return null;
        }

        public static void ResetSelectionNode(List<Node> nodes)
        {
            for (int i = 0; i < nodes.Count; i++)
            {
                nodes[i].IsSelected = false;
                nodes[i].Click = false;
            }
        }
        public static void ResetSelectionEdge(List<Edge> edges)
        {
            for (int i = 0; i < edges.Count; i++)
            {
                edges[i].IsSelected = false;
            }
        }
    }
}
