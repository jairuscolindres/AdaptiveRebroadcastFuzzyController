using AdaptiveRebroadcastFuzzyController;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;


namespace AdaptiveRebroadcastFuzzyController
{
    public partial class Form1 : Form
    {
        private List<NodeVisual> nodeVisuals = new List<NodeVisual>();
        private List<VisualPacket> activeVisualPackets = new List<VisualPacket>();
        private List<(int a, int b)> edges = new List<(int a, int b)>();


        private Random placementRng = new Random();


        public Form1()
        {
            InitializeComponent();
        }

        private void btnInitialize_Click(object sender, EventArgs e)
        {
            int nodeCount = int.Parse(txtNodes.Text.Trim());
            int packetCount = int.Parse(txtPackets.Text.Trim());
            int radius = int.Parse(txtRadius.Text.Trim());

            nodeVisuals.Clear();
            edges.Clear();
            
            int canvasWidth = canvasPanel.Width;
            int canvasHeight = canvasPanel.Height;
            float margin = radius;

            for(int i = 0; i < nodeCount; i++)
            {
                float x = (float)(placementRng.NextDouble() * (canvasWidth - 2 * margin) + margin);
                float y = (float)(placementRng.NextDouble() * (canvasHeight - 2 * margin) + margin);
                nodeVisuals.Add(new NodeVisual(i, new PointF(x, y), radius));
            }

            for(int i = 0; i < nodeVisuals.Count; i++)
            {
                for(int j = i + 1; j < nodeVisuals.Count; j++)
                {
                    float dist = Distance(nodeVisuals[i].Position, nodeVisuals[j].Position);
                    float combinedRadius = nodeVisuals[i].Radius + nodeVisuals[j].Radius;
                    if(dist < combinedRadius)
                    {
                        edges.Add((nodeVisuals[i].Id, nodeVisuals[j].Id));
                    }
                }
            }

            for(int p = 0; p < packetCount; p++)
            {
                int targetNodeIndex = placementRng.Next(nodeVisuals.Count);
                nodeVisuals[targetNodeIndex].Logic.ReceivePacket(p);
            }

            canvasPanel.Invalidate();
        }
        private float Distance(PointF a, PointF b)
        {
            float dx = a.X - b.X;
            float dy = a.Y - b.Y;
            return (float)Math.Sqrt(dx*dx + dy * dy);
        }

        private void canvasPanel_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;

            // 1. Draw Network Edges
            using (Pen edgePen = new Pen(Color.LightGray, 1))
            {
                foreach (var (a, b) in edges)
                {
                    var nodeA = nodeVisuals[a];
                    var nodeB = nodeVisuals[b];
                    g.DrawLine(edgePen, nodeA.Position, nodeB.Position);
                }
            }

            // 2. Draw Nodes
            foreach (var node in nodeVisuals)
            {
                int trackedCount = node.Logic.TrackedPacketCount;
                int collectedCount = node.Logic.CollectedPacketCount;

                bool hasActiveMemory = trackedCount > 0;
                bool hasStorage = collectedCount > 0;

                // Node Dot Color: Orange-Red when processing, Green when done, Blue if untouched
                Color nodeColor = hasActiveMemory ? Color.OrangeRed : (hasStorage ? Color.ForestGreen : Color.DarkBlue);

                // Fixed neutral color for the transmission radius (e.g., subtle gray/blue)
                Color radiusColor = Color.FromArgb(160, 190, 210);

                // Draw transmission radius circle with neutral styling
                using (Brush radiusBrush = new SolidBrush(Color.FromArgb(15, radiusColor)))
                using (Pen radiusPen = new Pen(Color.FromArgb(50, radiusColor), 1))
                {
                    float rx = node.Position.X - node.Radius;
                    float ry = node.Position.Y - node.Radius;
                    float diameter = node.Radius * 2;
                    g.FillEllipse(radiusBrush, rx, ry, diameter, diameter);
                    g.DrawEllipse(radiusPen, rx, ry, diameter, diameter);
                }

                // Draw node center dot (expands slightly when active)
                float dotSize = hasActiveMemory ? 10f : 6f;
                using (Brush dotBrush = new SolidBrush(nodeColor))
                {
                    g.FillEllipse(dotBrush,
                        node.Position.X - dotSize / 2,
                        node.Position.Y - dotSize / 2,
                        dotSize, dotSize);
                }

                // Draw node ID and packet stats label
                string label = $"{node.Id} [{trackedCount}] [{collectedCount}]";
                g.DrawString(label, Font, Brushes.Black, node.Position.X + 8, node.Position.Y - 6);
            }

            // 3. Draw Traveling Packets
            foreach (var vp in activeVisualPackets)
            {
                PointF pos = vp.CurrentPosition;
                float packetSize = 10f;
                
                using (Brush packetBrush = new SolidBrush(Color.Red))
                using (Pen borderPen = new Pen(Color.White, 1))
                {
                    g.FillEllipse(packetBrush, pos.X - packetSize / 2, pos.Y - packetSize / 2, packetSize, packetSize);
                    g.DrawEllipse(borderPen, pos.X - packetSize / 2, pos.Y - packetSize / 2, packetSize, packetSize);
                }
            }
        }
    
        private async void btnRun_Click(object sender, EventArgs e)
{
    // Run until NO node has any packets left in active memory (TrackedPacketCount > 0)
    while (nodeVisuals.Any(n => n.Logic.TrackedPacketCount > 0))
    {
        // 1. Evaluate fuzzy logic step for all nodes
        Dictionary<int, List<int>> broadcastsByNode = new Dictionary<int, List<int>>();
        foreach (var nodeVisual in nodeVisuals)
        {
            List<int> broadcastPackets = nodeVisual.Logic.RunStep();
            if (broadcastPackets.Count > 0)
            {
                broadcastsByNode[nodeVisual.Id] = broadcastPackets;
            }
        }

        // 2. Identify active edge transmissions and stage visual packets
        activeVisualPackets.Clear();
        List<(int receiverId, int packetId)> pendingArrivals = new List<(int, int)>();

        foreach (var (senderId, receiverId) in edges)
        {
            if (broadcastsByNode.ContainsKey(senderId))
            {
                foreach (int packetId in broadcastsByNode[senderId])
                {
                    activeVisualPackets.Add(new VisualPacket(nodeVisuals[senderId].Position, nodeVisuals[receiverId].Position));
                    pendingArrivals.Add((receiverId, packetId));
                }
            }

            if (broadcastsByNode.ContainsKey(receiverId))
            {
                foreach (int packetId in broadcastsByNode[receiverId])
                {
                    activeVisualPackets.Add(new VisualPacket(nodeVisuals[receiverId].Position, nodeVisuals[senderId].Position));
                    pendingArrivals.Add((senderId, packetId));
                }
            }
        }

        // 3. Animate packet progress along edges
        int animationFrames = 10;
        for (int frame = 0; frame <= animationFrames; frame++)
        {
            float progress = (float)frame / animationFrames;
            foreach (var vp in activeVisualPackets)
            {
                vp.Progress = progress;
            }

            canvasPanel.Refresh();
            await Task.Delay(20); // Keep this uncommented for visual feedback!
        }

        // 4. Deliver packets to receiver nodes after travel animation finishes
        foreach (var (receiverId, packetId) in pendingArrivals)
        {
            nodeVisuals[receiverId].Logic.ReceivePacket(packetId);
        }

        activeVisualPackets.Clear();
        canvasPanel.Refresh();
    }
}
    }

    public class NodeVisual
    {
        public int Id;
        public PointF Position;
        public float Radius;
        public Node Logic;
        public NodeVisual(int id, PointF position, float radius)
        {
            this.Id = id;
            this.Position = position;
            this.Radius = radius;
            Logic = new Node($"Node: {id}");
        }
    }
    public class VisualPacket
    {
        public PointF StartPos { get; set; }
        public PointF EndPos { get; set; }
        public float Progress { get; set; } = 0f; // Range from 0.0 (sender) to 1.0 (receiver)

        public PointF CurrentPosition => new PointF(
            StartPos.X + (EndPos.X - StartPos.X) * Progress,
            StartPos.Y + (EndPos.Y - StartPos.Y) * Progress
        );

        public VisualPacket(PointF start, PointF end)
        {
            StartPos = start;
            EndPos = end;
        }
    }
}
