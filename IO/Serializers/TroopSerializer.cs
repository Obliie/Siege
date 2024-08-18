using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Xna.Framework;
using Siege.Core;
using Siege.Entities;

namespace Siege.IO.Serializers {
    public class TroopSerializer : Serializable<TroopBase> {
        public FileNode Serialize(TroopBase obj) {
            FileNode Node = new FileNode();

            Node.SetValue<TroopType>("TroopType", obj.GetTroopType());
            Node.SetValue<float>("Health", obj.Health);
            Node.SetValue<bool>("Friendly", obj.Friendly);
            Node.SetValue<Vector2>("Position", obj.Position);
            Node.SetValue<string>("MapUID", obj.map.UID.ToString());

            return Node;
        }

        public TroopBase Deserialize(FileNode node) {
            MapBase Map = null;
            string MapUID = node.GetValue<string>("MapUID");
            if (Siege.INSTANCE.MapService.CurrentMap.UID.ToString().Equals(MapUID)) {
                Map = Siege.INSTANCE.MapService.CurrentMap;
            }

            if (Map == null) {
                throw new NodeException.FailedDependantDeserialization();
            }

            TroopType TType = node.GetValue<TroopType>("TroopType");
            float Health = node.GetValue<float>("Health");
            bool Friendly = node.GetValue<bool>("Friendly");
            Vector2 Position = node.GetValue<Vector2>("Position");
            TroopBase Troop = TroopBuilder.Builder()
                .Map(Map)
                .Position(Position)
                .Friendly(Friendly)
                .Type(TType)
                .Health(Health)
                .Build();

            return Troop;
        }
    }
}
