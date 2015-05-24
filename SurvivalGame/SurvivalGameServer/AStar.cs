using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Linq;

namespace Mentula.SurvivalGameServer
{
    public static class AStar
    {
        public const int MOVE = 10;

        private static Map _map;
        private static List<Vector2> _open;
        private static List<Vector2> _closed;

        public static Node[] GetRoute(Map map)
        {
            _map = map;
            _open = new List<Vector2>();
            _closed = new List<Vector2>();
            Node current = _map.GetStartNode();

            while (true)
            {
                if (_open.Contains(current.Position)) _open.Remove(current.Position);
                _closed.Add(current.Position);

                Vector2[] ajNodes = FilterNodes(GetAjasonNodes(current.Position));
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

        private static Node[] GetAjasonNodes(Vector2 nodePos)
        {
            List<Node> returnV = new List<Node>();
            Rectangle dim = _map.GetDim();

            if (nodePos.X - 1 >= dim.X) returnV.Add(_map[nodePos.X - 1, nodePos.Y]);
            if (nodePos.X + 1 < dim.Width) returnV.Add(_map[nodePos.X + 1, nodePos.Y]);
            if (nodePos.Y - 1 >= dim.Y) returnV.Add(_map[nodePos.X, nodePos.Y - 1]);
            if (nodePos.Y + 1 < dim.Height) returnV.Add(_map[nodePos.X, nodePos.Y + 1]);

            return returnV.ToArray();
        }

        private static Vector2[] FilterNodes(Node[] ajasonNodes)
        {
            List<Vector2> result = new List<Vector2>();

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

                if (min != cur & cur.FValue < min.FValue) min = cur;
            }

            return min;
        }

        private static List<Node> GetNodes()
        {
            List<Node> result = new List<Node>();

            for (int i = 0; i < _open.Count; i++)
            {
                Vector2 cur = _open[i];

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

        public class Node
        {
            public Vector2 Position { get; private set; }
            public int Heuristic { get; private set; }

            public int FValue { get { return GValue + Heuristic; } }
            public Node Parent { get { return _parent; } }

            public bool wall;
            public int GValue;

            private Node _parent;

            public Node(Vector2 position)
            {
                Position = position;
            }

            public Node(Vector2 position, int g)
            {
                Position = position;
                GValue = g;
            }

            public Node(Vector2 position, int g, bool pathable)
            {
                Position = position;
                GValue = g;
                wall = !pathable;
            }

            public void SetHeuristic(Vector2 endPoint)
            {
                Heuristic = (int)Vector2.Distance(Position, endPoint);
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
            public Vector2 startPos { get; private set; }
            public Vector2 endPos { get; private set; }

            private int _width;
            private int _height;

            public Map(int width, int height)
            {
                _width = width;
                _height = height;
                EmptyMap();
            }

            public Map(int width, int height, Vector2 start, Vector2 end)
            {
                _width = width;
                _height = height;
                EmptyMap();
                startPos = start;
                endPos = end;
                SetHeuristic();
            }

            public Map(int width, Vector2 start, Vector2 end, Node[] map)
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

            public void SetStart(Vector2 pos)
            {
                startPos = pos;
            }

            public void SetEnd(Vector2 pos)
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
                        nodes[x, y] = new Node(new Vector2(x, y));
                    }
                }

                startPos = -Vector2.One;
                endPos = -Vector2.One;
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