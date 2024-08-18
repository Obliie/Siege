using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;

namespace Siege.IO.Serializers {
    public class Vector2Serializer : Serializable<Vector2> {
        public FileNode Serialize(Vector2 obj) {
            FileNode Node = new FileNode();

            Node.SetValue<float>("X", obj.X);
            Node.SetValue<float>("Y", obj.Y);

            return Node;
        }

        public Vector2 Deserialize(FileNode node) {
            float X = node.GetValue<float>("X");
            float Y = node.GetValue<float>("Y");

            Vector2 Vector = new Vector2(X, Y);

            return Vector;
        }
    }
}
