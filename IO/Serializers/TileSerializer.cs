using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Siege.Map.Tiles;

namespace Siege.IO.Serializers {
    public class TileSerializer : Serializable<Tile> {
        public FileNode Serialize(Tile obj) {
            FileNode Node = new FileNode();

            Node.SetValue<int>("X", obj.X);
            Node.SetValue<int>("Y", obj.Y);
            Node.SetValue<Rectangle>("Bounds", obj.Bounds);

            Node.SetValue<bool>("AcceptsTE", obj.Properties.AcceptsTileEntity);
            Node.SetValue<float>("DamageTick", obj.Properties.DamageTick);
            Node.SetValue<bool>("Modifyable", obj.Properties.Modifyable);
            Node.SetValue<bool>("Solid", obj.Properties.Solid);
            Node.SetValue<float>("SpeedModifier", obj.Properties.SpeedModifier);

            if (obj.Properties.TextureID.HasValue) {
                Node.SetValue<int>("TextureID", obj.Properties.TextureID.Value);
            }

            Node.SetValue<bool>("TroopPath", obj.Properties.TroopPath);

            return Node;
        }

        public Tile Deserialize(FileNode node) {
            int X = node.GetValue<int>("X");
            int Y = node.GetValue<int>("Y");
            Rectangle Bounds = node.GetValue<Rectangle>("Bounds");

            Tile tile = new Tile(X, Y, Bounds);
            tile.Properties.AcceptsTileEntity = node.GetValue<bool>("AcceptsTE");
            tile.Properties.DamageTick = node.GetValue<float>("DamageTick");
            tile.Properties.Modifyable = node.GetValue<bool>("Modifyable");
            tile.Properties.Solid = node.GetValue<bool>("Solid");
            tile.Properties.SpeedModifier = node.GetValue<float>("SpeedModifier");

            if (node.ContainsValue("TextureID")) {
                tile.Properties.TextureID = node.GetValue<int>("TextureID");
            }

            tile.Properties.TroopPath = node.GetValue<bool>("TroopPath");

            return tile;
        }
    }
}
