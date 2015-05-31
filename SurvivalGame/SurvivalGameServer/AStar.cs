using Mentula.General;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace Mentula.SurvivalGameServer
{
    public static class AStar
    {
        public const int MOVE = 10;

        private static Map _map;
        private static List<IntVector2> _open;
        private static List<IntVector2> _closed;

        public static Node[] GetRoute(Map map)
        {
            _map = map;
            _open = new List<IntVector2>();
            _closed = new List<IntVector2>();
            Node current = _map.GetStartNode();

            while (true)
            {
                if (_open.Contains(current.Position)) _open.Remove(current.Position);
                _closed.Add(current.Position);

                IntVector2[] ajNodes = FilterNodes(GetAjasonNodes(current.Position));
                if (ajNodes.Contains(map.endPos)) break;

                for (int i = 0; i < ajNodes.Length; i++)
                {
                    Node cur = _map[ajNodes[i].X, ajNodes[i].Y];
                    if (cur.Parent == null) cur.SetParent(current, MOVE);
                    if (current.GValue + MOVE < cur.GValue) cur.SetParent(current, MOVE);
                    if (!_open.Contains(cur.Position)) _open.Add(cur.Position);
                }

                if (_open.Count == 0) return new Node[0];
                current = GetNodeWithLowestFV(GetNodes());
            }

            Node end = map.GetEndNode();
            end.SetParent(current, MOVE);
            return Callback(ref end);
        }

        private static Node[] GetAjasonNodes(IntVector2 nodePos)
        {
            List<Node> returnV = new List<Node>();
            Rectangle dim = _map.GetDim();

            if (nodePos.X - 1 >= dim.X) returnV.Add(_map[nodePos.X - 1, nodePos.Y]);
            if (nodePos.X + 1 < dim.Width) returnV.Add(_map[nodePos.X + 1, nodePos.Y]);
            if (nodePos.Y - 1 >= dim.Y) returnV.Add(_map[nodePos.X, nodePos.Y - 1]);
            if (nodePos.Y + 1 < dim.Height) returnV.Add(_map[nodePos.X, nodePos.Y + 1]);

            return returnV.ToArray();
        }

        private static IntVector2[] FilterNodes(Node[] ajasonNodes)
        {
            List<IntVector2> result = new List<IntVector2>();

            for (int i = 0; i < ajasonNodes.Length; i++)
            {
                Node cur = ajasonNodes[i];

                if (!cur.wall & !_closed.Contains(cur.Position)) result.Add(cur.Position);
            }

            return result.ToArray();
        }

        private static Node GetNodeWithLowestFV(List<Node> openList)
        {
            Node min = openList[0];

            for (int i = 0; i < openList.Count; i++)
            {
                Node cur = openList[i];

                if (min.Position != cur.Position & cur.FValue < min.FValue) min = cur;
            }

            return min;
        }

        private static List<Node> GetNodes()
        {
            List<Node> result = new List<Node>();

            for (int i = 0; i < _open.Count; i++)
            {
                IntVector2 cur = _open[i];

                result.Add(_map[cur.X, cur.Y]);
            }

            return result;
        }

        private static Node[] Callback(ref Node endPos)
        {
            List<Node> result = new List<Node>();
            Node current = endPos;

            while (current.Parent != null)
            {
                result.Add(current);
                current = current.Parent;
            }

            result.RemoveAt(0);

            return result.ToArray();
        }

        [DebuggerDisplay("Pos={Position}")]
        public class Node
        {
            public IntVector2 Position { get; private set; }
            public int Heuristic { get; private set; }

            public int FValue { get { return GValue + Heuristic; } }
            public Node Parent { get { return _parent; } }

            public bool wall;
            public int GValue;

            private Node _parent;

            public Node(IntVector2 position)
            {
                Position = position;
            }

            public Node(IntVector2 position, int g)
            {
                Position = position;
                GValue = g;
            }

            public Node(IntVector2 position, int g, bool pathable)
            {
                Position = position;
                GValue = g;
                wall = !pathable;
            }

            public void SetHeuristic(IntVector2 endPoint)
            {
                Heuristic = IntVector2.Distance(Position, endPoint);
            }

            public void SetParent(Node parent, int move)
            {
                GValue += parent.GValue + move;
                _parent = parent;
            }
        }

        public class Map
        {
            public Node[,] nodes;
            public IntVector2 startPos { get; private set; }
            public IntVector2 endPos { get; private set; }

            private int _width;
            private int _height;

            public Map(int width, int height)
            {
                _width = width;
                _height = height;
                EmptyMap();
            }

            public Map(int width, int height, IntVector2 start, IntVector2 end)
            {
                _width = width;
                _height = height;
                EmptyMap();
                startPos = start;
                endPos = end;
                SetHeuristic();
            }

            public Map(int width, IntVector2 start, IntVector2 end, Node[] map)
            {
                _width = width;
                _height = map.Length / width;
                EmptyMap();

                for (int y = 0; y < _height; y++)
                {
                    for (int x = 0; x < _width; x++)
                    {
                        nodes[x, y] = map[(y * _width) + x];
                    }
                }

                startPos = start;
                endPos = end;
                SetHeuristic();
            }

            public Node this[float x, float y]
            {
                get
                {
                    return nodes[(int)x, (int)y];
                }
                set
                {
                    nodes[(int)x, (int)y] = value;
                }
            }

            public void SetStart(IntVector2 pos)
            {
                startPos = pos;
            }

            public void SetEnd(IntVector2 pos)
            {
                endPos = pos;
                SetHeuristic();
            }

            public void EmptyMap()
            {
                nodes = new Node[_width, _height];
                for (int y = 0; y < _height; y++)
                {
                    for (int x = 0; x < _width; x++)
                    {
                        nodes[x, y] = new Node(new IntVector2(x, y));
                    }
                }

                startPos = -IntVector2.One;
                endPos = -IntVector2.One;
            }

            private void SetHeuristic()
            {
                foreach (Node cur in nodes)
                {
                    cur.SetHeuristic(endPos);
                }
            }

            public Rectangle GetDim()
            {
                return new Rectangle(0, 0, _width, _height);
            }

            public Node GetStartNode()
            {
                return nodes[(int)startPos.X, (int)startPos.Y];
            }

            public Node GetEndNode()
            {
                return nodes[(int)endPos.X, (int)endPos.Y];
            }
        }
    }
}