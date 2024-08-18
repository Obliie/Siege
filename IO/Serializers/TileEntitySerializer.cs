using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Siege.Core;
using Siege.Tower;

namespace Siege.IO.Serializers {
    public class TileEntitySerializer : Serializable<ITileEntity> {
        public FileNode Serialize(ITileEntity obj) {
            FileNode Node = new FileNode();

            if (obj is TowerBase) {
                TowerBase Tower = obj as TowerBase;
                return SerializerService.GetSerializer<TowerBase>().Serialize(Tower);
            }

            throw new NodeException.NotSerializable();
        }

        public ITileEntity Deserialize(FileNode node) {
            if (node.ContainsValue("TowerType")) {
                return SerializerService.GetSerializer<TowerBase>().Deserialize(node);
            }

            throw new NodeException.NotSerializable();
        }
    }
}
