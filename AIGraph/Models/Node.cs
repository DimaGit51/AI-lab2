using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AIGraph.Models
{
    public class Node
    {
        private string name;
        private Point position;
        private bool isSelected;
        private bool click;
        private double weight;
        private int radius = 40; // стандартный размер

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        public Point Position
        {
            get { return position; }
            set { position = value; }
        }

        public bool IsSelected
        {
            get { return isSelected; }
            set { isSelected = value; }
        }

        public bool Click
        {
            get { return click; }
            set { click = value; }
        }
        public double Weight
        {
            get { return weight; }
            set { weight = value; }
        }
        public int Radius
        {
            get { return radius; }
            set { radius = Math.Max(5, value); } // минимальный размер 5
        }

        public Node(string name, Point position, double weight = 0, int radius = 40)
        {
            this.name = name;
            this.position = position;
            this.isSelected = false;
            this.click = false;
            this.weight = weight;
            this.radius = radius;
        }
    }
}
