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

        public Node(string name, Point position)
        {
            this.name = name;
            this.position = position;
            this.isSelected = false;
        }
    }
}
