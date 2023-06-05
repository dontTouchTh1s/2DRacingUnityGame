using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

namespace Cops.AStar
{
    public class Node
    {
        public readonly List<Node> Neighbors = new();

        public int GCost;
        public int HCost;
        public bool IsCostCalculated;
        public bool IsObstacle = false;
        public int PickOrder;
        public Vector2Int Position;
        public int TotalCost;

        public Node(Vector2Int gridPosition)
        {
            Position = gridPosition;
        }

        public void CalculateCost(Node start, Node target)
        {
            var nodePosition = Position;
            var startNodePosition = start.Position;
            var targetNodePosition = target.Position;

            GCost = math.abs(startNodePosition.x - nodePosition.x) +
                    math.abs(startNodePosition.y - nodePosition.y);
            HCost = math.abs(targetNodePosition.x - nodePosition.x) +
                    math.abs(targetNodePosition.y - nodePosition.y);
            TotalCost = GCost + HCost;
            IsCostCalculated = true;
        }

        public void Clear()
        {
            GCost = 0;
            HCost = 0;
            TotalCost = 0;
            IsCostCalculated = false;
        }
    }
}