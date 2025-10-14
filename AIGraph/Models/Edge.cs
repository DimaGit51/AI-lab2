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
                if (value < -1)
                {
                    weight = -1;
                    MessageBox.Show("Вес не может быть меньше -1. Установлено значение -1.",
                                    "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else if (value > 1)
                {
                    weight = 1;
                    MessageBox.Show("Вес не может быть больше 1. Установлено значение 1.",
                                    "Предупреждение", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                {
                    weight = value;
                }
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
