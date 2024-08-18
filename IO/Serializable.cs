using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Siege.IO {
    /// <summary>
    /// Defined to allow the generic Serializable type to be stored in a dictionary for easy
    /// management, this interface should NOT be implemented.
    /// </summary>
    public interface Serializable { }

    public interface Serializable<T> : Serializable {
        /// <summary>
        /// Serializes an object into a FileNode which can then be written to a file.
        /// </summary>
        /// <param name="obj">The object to serialize.</param>
        /// <returns>The serialized FileNode.</returns>
        FileNode Serialize(T obj);

        /// <summary>
        /// Deserializes an object from a FileNode back into its original form.
        /// </summary>
        /// <param name="node">The FileNode to deserialize.</param>
        /// <returns>The deserialized object.</returns>
        T Deserialize(FileNode node);
    }
}
