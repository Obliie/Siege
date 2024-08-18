using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Siege.Core;
using Siege.Map.Tiles;
using Siege.Tower;

namespace Siege.IO.Serializers {
    public class TowerSerializer : Serializable<TowerBase> {
        public FileNode Serialize(TowerBase obj) {
            FileNode Node = new FileNode();

            Node.SetValue<TowerType>("TowerType", obj.GetTowerType());
            Node.SetValue<bool>("Friendly", obj.Friendly);
            Node.SetValue<Tile>("Position", obj.GetTilePosition());
            Node.SetValue<string>("MapUID", obj.map.UID.ToString());

            return Node;
        }

        public TowerBase Deserialize(FileNode node) {
            MapBase Map = null;
            string MapUID = node.GetValue<string>("MapUID");
            if (Siege.INSTANCE.MapService.CurrentMap.UID.ToString().Equals(MapUID)) {
                Map = Siege.INSTANCE.MapService.CurrentMap;
            }

            if (Map == null) {
                throw new NodeException.FailedDependantDeserialization();
            }

            Tile Position = node.GetValue<Tile>("Position");
            bool Friendly = node.GetValue<bool>("Friendly");
            TowerType Type = node.GetValue<TowerType>("TowerType");
            TowerBase Tower = TowerBuilder.Builder()
                .Map(Map)
                .Position(Map.GetRowAtY(Position.Y).GetTileAtX(Position.X))
                .Friendly(Friendly)
                .Type(Type)
                .Build();

            return Tower;
        }
    }
}
