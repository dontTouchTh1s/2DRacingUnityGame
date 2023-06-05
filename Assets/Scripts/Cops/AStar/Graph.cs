using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace Cops.AStar
{
    public class Graph
    {
        private readonly float _cellSize;
        private readonly int _height;
        private readonly int _width;
        private int _attempts;
        private List<Node> _explored;
        private bool _finished;
        private List<Node> _path;
        private int _pickOrder;
        private List<Node> _reachable;
        private Node _start;
        private Node _target;
        public Node[,] Nodes;

        public Graph(int width, int height, float cellSize)
        {
            _width = width;
            _height = height;
            _cellSize = cellSize;
            Nodes = new Node[_width, _height];
            CreateGrid();
        }

        private void CreateGrid()
        {
            for (var x = 0; x < _width; x++)
            for (var y = 0; y < _height; y++)
            {
                var node = new Node(new Vector2Int(x, y));
                Nodes[x, y] = node;
                // Check if node is in an obstacle
                var worldPos = GridPosToWorldPos(node);
                var overlapCircle = Physics2D.OverlapCircle(worldPos, _cellSize / 2);
                if (overlapCircle == null || overlapCircle.isTrigger) continue;
                if (overlapCircle.CompareTag("CarsCollider") || overlapCircle.CompareTag("PlayerCollider"))
                    continue;
                node.IsObstacle = true;
            }

            foreach (var node in Nodes)
            {
                var nodePosition = node.Position;
                if (nodePosition.x != 0)
                {
                    var neighborNode = Nodes[nodePosition.x - 1, nodePosition.y];
                    if (!neighborNode.IsObstacle)
                        node.Neighbors.Add(neighborNode);
                }

                if (nodePosition.y != 0)
                {
                    var neighborNode = Nodes[nodePosition.x, nodePosition.y - 1];
                    if (!neighborNode.IsObstacle)
                        node.Neighbors.Add(neighborNode);
                }

                if (nodePosition.x < _width - 1)
                {
                    var neighborNode = Nodes[nodePosition.x + 1, nodePosition.y];
                    if (!neighborNode.IsObstacle)
                        node.Neighbors.Add(neighborNode);
                }

                if (nodePosition.y < _height - 1)
                {
                    var neighborNode = Nodes[nodePosition.x, nodePosition.y + 1];
                    if (!neighborNode.IsObstacle)
                        node.Neighbors.Add(neighborNode);
                }
            }
        }

        private Node WorldPosToGridPos(Vector2 position)
        {
            var nodePos = new Vector2Int(Convert.ToInt32(position.x / _cellSize),
                Convert.ToInt32(position.y / _cellSize));
            return Nodes[nodePos.x, nodePos.y];
        }

        private Vector2 GridPosToWorldPos(Node node)
        {
            var nodePos = node.Position;
            return new Vector2(nodePos.x * _cellSize + _cellSize / 2, nodePos.y * _cellSize + _cellSize / 2);
        }

        public Task<List<Vector2>> FindPath(Vector2 start, Vector2 target)
        {
            return Task.Run(() =>
                {
                    var startNode = WorldPosToGridPos(start);
                    var targetNode = WorldPosToGridPos(target);

                    _start = startNode;
                    _target = targetNode;
                    _reachable = new List<Node> { startNode };
                    _explored = new List<Node>();
                    _path = new List<Node>();
                    _finished = false;
                    _attempts = 0;
                    _pickOrder = 0;
                    Clear();
                    _start.CalculateCost(_start, _target);
                    var node = _start;
                    {
                        while (!_finished)
                        {
                            _attempts++;
                            _reachable.Remove(node);
                            node.PickOrder = _pickOrder;
                            _pickOrder++;
                            _explored.Add(node);

                            if (node.Position == _target.Position)
                            {
                                _finished = true;
                                Debug.Log(_attempts);
                                break;
                            }

                            if (_attempts > 3000)
                            {
                                _finished = true;
                                Debug.Log(_attempts);
                                break;
                            }

                            // Calculate cost if target is not neighbor is not obstacle and not calculated before 
                            foreach (var nodeNeighbor in node.Neighbors)
                                if (!nodeNeighbor.IsCostCalculated)
                                    nodeNeighbor.CalculateCost(_start, _target);

                            foreach (var nodeNeighbor in node.Neighbors)
                            {
                                if (_explored.Contains(nodeNeighbor)) continue;
                                if (_reachable.Contains(nodeNeighbor)) continue;
                                _reachable.Add(nodeNeighbor);
                            }

                            _reachable = _reachable.OrderBy(n => n.TotalCost).ThenBy(n => n.HCost).ToList();

                            if (_reachable.Count == 0)
                            {
                                Debug.LogWarning("No nodes left in next nodes to check, we have no solution");
                                return null;
                            }

                            node = _reachable[0];
                        }
                    }
                    CalculatePath();

                    var vectorPath = new List<Vector2>();
                    foreach (var n in _path) vectorPath.Add(GridPosToWorldPos(n));

                    return vectorPath;
                }
            );
        }

        private void Clear()
        {
            foreach (var node in Nodes) node.Clear();
        }

        private void CalculatePath()
        {
            _explored.Reverse();
            var currentNode = _explored[0];
            var pathCreated = false;
            _path.Add(currentNode);
            var attempts = 0;
            while (!pathCreated)
            {
                var orderedNeighbor = currentNode.Neighbors.OrderBy(n => n.PickOrder).ToList();
                foreach (var orderedNode in orderedNeighbor)
                    if (_explored.Contains(orderedNode) && !_path.Contains(orderedNode))
                    {
                        _path.Add(orderedNode);
                        currentNode = orderedNode;
                    }

                if (currentNode == _start)
                    pathCreated = true;
                if (attempts > 1000) break;
                attempts++;
            }
        }
    }
}