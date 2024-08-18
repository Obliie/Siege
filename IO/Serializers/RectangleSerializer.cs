using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace Siege.IO.Serializers {
    public class RectangleSerializer : Serializable<Rectangle> {
        public FileNode Serialize(Rectangle obj) {
            FileNode Node = new FileNode();

            Node.SetValue<int>("X", obj.X);
            Node.SetValue<int>("Y", obj.Y);
            Node.SetValue<int>("Width", obj.Width);
            Node.SetValue<int>("Height", obj.Height);

            return Node;
        }

        public Rectangle Deserialize(FileNode node) {
            int X = node.GetValue<int>("X");
            int Y = node.GetValue<int>("Y");
            int Width = node.GetValue<int>("Width");
            int Height = node.GetValue<int>("Height");

            Rectangle rectangle = new Rectangle(X, Y, Width, Height);
            return rectangle;
        }
    }
}
