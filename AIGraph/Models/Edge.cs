using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIGraph.Models
{
    public class Edge
    {
        private Node from;
        private Node to;
        private double weight;
        private bool isSelected;

        public Node From
        {
            get { return from; }
            set { from = value; }
        }

        public Node To
        {
            get { return to; }
            set { to = value; }
        }

        public double Weight
        {
            get { return weight; }
            set
            {
                // Ограничиваем вес значениями от -1 до 1
                if (value < -1) weight = -1;
                else if (value > 1) weight = 1;
                else weight = value;
            }
        }

        public bool IsSelected
        {
            get { return isSelected; }
            set { isSelected = value; }
        }

        public Edge(Node from, Node to, double weight = 1)
        {
            this.from = from;
            this.to = to;
            this.Weight = weight; // используем сеттер с ограничением
            this.isSelected = false;
        }
    }
}
