using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Siege.Core;
using Siege.Entities;

namespace Siege.IO.Serializers {
    public class PlayerSerializer : Serializable<Player> {
        public FileNode Serialize(Player obj) {
            FileNode Node = new FileNode();

            Node.SetValue<Vector2>("Position", obj.Position);
            Node.SetValue<string>("MapUID", obj.map.UID.ToString());

            return Node;
        }

        public Player Deserialize(FileNode node) {
            MapBase Map = null;
            string MapUID = node.GetValue<string>("MapUID");
            if (Siege.INSTANCE.MapService.CurrentMap.UID.ToString().Equals(MapUID)) {
                Map = Siege.INSTANCE.MapService.CurrentMap;
            }

            if (Map == null) {
                throw new NodeException.FailedDependantDeserialization();
            }

            Vector2 Position = node.GetValue<Vector2>("Position");
            Player Player = new Player(Map, Position);
            Player.PlayerTexture = Siege.PlayerTexture;

            return Player;
        }
    }
}
