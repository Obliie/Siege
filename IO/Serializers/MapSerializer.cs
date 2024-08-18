using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Siege.Core;
using Siege.Entities;
using Siege.Map.Maps;
using Siege.Map.Tiles;
using Siege.Tower;

namespace Siege.IO.Serializers {
    public class MapSerializer : Serializable<MapBase> {
        public FileNode Serialize(MapBase obj) {
            FileNode Node = new FileNode();

            Node.SetList<ITileEntity>("TileEntities", obj.GetTileEntities().Values.ToList());
            Node.SetList<TroopBase>("Troops", obj.GetTroops());
            Node.SetValue<Player>("Player", obj.player);

            Node.SetValue<string>("MapUID", obj.UID.ToString());
            Node.SetValue<int>("XOffset", obj.XOffset);
            Node.SetValue<int>("YOffset", obj.YOffset);

            Node.SetValue<MapType>("MapType", obj.GetMapType());

            return Node;
        }

        public MapBase Deserialize(FileNode node) {
            MapType mapType = node.GetValue<MapType>("MapType");
            string MapUID = node.GetValue<string>("MapUID");

            MapBase map = Siege.INSTANCE.MapService.LoadMap(mapType);
            map.UID = new Guid(MapUID);

            List<ITileEntity> TileEntities = node.GetList<ITileEntity>("TileEntities");
            List<TroopBase> Troops = node.GetList<TroopBase>("Troops");
            Player player = node.GetValue<Player>("Player");

            int XOffset = node.GetValue<int>("XOffset");
            int YOffset = node.GetValue<int>("YOffset");

            foreach(ITileEntity TE in TileEntities) {
                map.AddTileEntity(TE);
            }
            foreach (TroopBase Troop in Troops) {
                map.DeployTroop(Troop);
            }
            map.player = player;
            map.XOffset = XOffset;
            map.YOffset = YOffset;

            return map;
        }
    }
}
