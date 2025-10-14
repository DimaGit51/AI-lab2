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

        public Node(string name, Point position, double weight = 0)
        {
            this.name = name;
            this.position = position;
            this.isSelected = false;
            this.click = false;
            this.weight = weight;
        }
    }
}
