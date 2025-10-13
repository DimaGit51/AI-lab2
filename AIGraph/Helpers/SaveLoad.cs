using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using AIGraph.Models;

namespace AIGraph.Helpers
{
    [Serializable]
    public class GraphData
    {
        public List<NodeData> Nodes { get; set; }
        public List<EdgeData> Edges { get; set; }
    }

    [Serializable]
    public class NodeData
    {
        public string Name { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
    }

    [Serializable]
    public class EdgeData
    {
        public string From { get; set; }
        public string To { get; set; }
        public double Weight { get; set; }
    }

    public static class SaveLoad
    {
        public static void SaveGraph(string path, List<Node> nodes, List<Edge> edges)
        {
            GraphData data = new GraphData
            {
                Nodes = new List<NodeData>(),
                Edges = new List<EdgeData>()
            };

            foreach (var n in nodes)
                data.Nodes.Add(new NodeData { Name = n.Name, X = n.Position.X, Y = n.Position.Y });

            foreach (var e in edges)
                data.Edges.Add(new EdgeData { From = e.From.Name, To = e.To.Name, Weight = e.Weight });

            using (FileStream fs = new FileStream(path, FileMode.Create))
            using (BinaryWriter writer = new BinaryWriter(fs))
            {
                writer.Write(data.Nodes.Count);
                foreach (var n in data.Nodes)
                {
                    writer.Write(n.Name);
                    writer.Write(n.X);
                    writer.Write(n.Y);
                }

                writer.Write(data.Edges.Count);
                foreach (var e in data.Edges)
                {
                    writer.Write(e.From);
                    writer.Write(e.To);
                    writer.Write(e.Weight);
                }
            }
        }

        public static void LoadGraph(string path, out List<Node> nodes, out List<Edge> edges)
        {
            nodes = new List<Node>();
            edges = new List<Edge>();

            using (FileStream fs = new FileStream(path, FileMode.Open))
            using (BinaryReader reader = new BinaryReader(fs))
            {
                int nodeCount = reader.ReadInt32();
                for (int i = 0; i < nodeCount; i++)
                {
                    string name = reader.ReadString();
                    int x = reader.ReadInt32();
                    int y = reader.ReadInt32();
                    nodes.Add(new Node(name, new Point(x, y)));
                }

                int edgeCount = reader.ReadInt32();
                for (int i = 0; i < edgeCount; i++)
                {
                    string from = reader.ReadString();
                    string to = reader.ReadString();
                    double weight = reader.ReadDouble();

                    Node fromNode = nodes.Find(n => n.Name == from);
                    Node toNode = nodes.Find(n => n.Name == to);
                    if (fromNode != null && toNode != null)
                        edges.Add(new Edge(fromNode, toNode, weight));
                }
            }
        }
    }
}
