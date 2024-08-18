using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Siege.IO {
    public class FileNode {
        // Declare a dictionary called nodeData which stores all the related data for this node.
        // A key is connected to an Object of data.
        private Dictionary<string, Object> nodeData = new Dictionary<string, Object>();

        /// <summary>
        /// A constructor used to create a fresh File Node with no stored data.
        /// </summary>
        public FileNode() { }

        /// <summary>
        /// A constructor used to create a FileNode with a predefined set of data.
        /// </summary>
        /// <param name="nodeData">The data to create the FileNode with.</param>
        public FileNode(Dictionary<string, Object> nodeData) {
            this.nodeData = nodeData;
        }

        /// <summary>
        /// Attempts to retrieve a child FileNode at the supplied location.
        /// </summary>
        /// <param name="key">The key to retrieve the child node.</param>
        /// <returns>The child node if found, else an exception is thrown.</returns>
        public FileNode GetNode(string key) {
            // Check if the key exists within this nodes data.
            if (!nodeData.ContainsKey(key)) { 
                throw new NodeException.NotFound();
            }

            // Attempt to get the Node stored at the keys location.
            Object value;
            nodeData.TryGetValue(key, out value);

            // Check if the retrieved Object is actually a Node.
            if (!value.GetType().Equals(typeof(FileNode))) {
                throw new NodeException.IncorrectType();
            }

            // Return the Node.
            return (FileNode) value;
        }

        /// <summary>
        /// Attempts to retrieve a child FileNode at the supplied location, if the
        /// node doesn't exist it will be created.
        /// </summary>
        /// <param name="key">The key to retrieve the child node.</param>
        /// <returns>The child node if found or created, else an exception is thrown.</returns>
        public FileNode GetOrCreateNode(string key) {
            // Check if the key exists within this nodes data.
            if (!nodeData.ContainsKey(key)) {
                // A Node doesn't exist so create one and return it.
                FileNode newNode = new FileNode(); 
                nodeData.Add(key, newNode); 
                return newNode;
            }

            // Attempt to get the Node stored at the keys location.
            Object value;
            nodeData.TryGetValue(key, out value);

            // Check if the retrieved Object is actually a Node.
            if (!value.GetType().Equals(typeof(FileNode))) {
                throw new NodeException.IncorrectType();
            }

            // Return the Node.
            return (FileNode)value;
        }

        public bool ContainsValue(string Key) {
            return nodeData.ContainsKey(Key);
        }

        /// <summary>
        /// Attempts to retrieve the value at the supplied location.
        /// </summary>
        /// <typeparam name="T">The type of the value to retrieve.</typeparam>
        /// <param name="key">The key to retrieve the value from.</param>
        /// <returns>The value if found, else an exception is thrown.</returns>
        public T GetValue<T>(string key) {
            // Check if the key exists within this nodes data. If it doesen't throw an exception.
            if (!nodeData.ContainsKey(key)) {
                throw new NodeException.NotFound();
            }

            // Attempt to get the Object stored at the keys location.
            Object value;
            nodeData.TryGetValue(key, out value);

            // If the object is of a serializable type we want to get its deserialized form to return
            // Request the serializer for the type and attempt to deserialize the Object.
            if (SerializerService.GetSerializer<T>() != null) {
                Serializable<T> serializer = SerializerService.GetSerializer<T>();
                return serializer.Deserialize((FileNode)Convert.ChangeType(value, typeof(FileNode)));
            }

            // Check if the retrieved value is of the type T. If it isn't throw an exception
            try {
                // Enums need special treatment... still checks if type is of T but using methods explicitly
                // defined for Enums.
                if (typeof(T).IsEnum) {
                    T converted = (T)Enum.Parse(typeof(T), (string)value, true);
                } else {
                    T converted = (T)Convert.ChangeType(value, typeof(T));
                }
            } catch (Exception ex) {
                throw new NodeException.IncorrectType();
            }

            // Return the requested value.
            if (typeof(T).IsEnum) {
                return (T)Enum.Parse(typeof(T), (string)value, true);
            } else {
                return (T)Convert.ChangeType(value, typeof(T));
            }
        }

        /// <summary>
        /// Attempts to retrieve a list at the supplied location
        /// </summary>
        /// <typeparam name="T">The type of the value in the list to retrieve.</typeparam>
        /// <param name="key">The key to retrieve the list from.</param>
        /// <returns>The list of data, this will be empty if no data is found.</returns>
        public List<T> GetList<T>(string key) {
            // Create a new list of the provided type and get the child node the list is saved in.
            List<T> List = new List<T>();
            FileNode ChildNode = GetOrCreateNode(key);

            // Iterate through each piece of data in the ChildNode and get the value (of type T) saved, add it to
            // the list.
            for (int i = 0; i < ChildNode.GetRawData().Keys.Count; i++) {
                T value = ChildNode.GetValue<T>(i.ToString());
                List.Insert(i, value);
            }

            // Return the list.
            return List;
        }

        /// <summary>
        /// Attempts to retrieve a dictionary at the supplied location
        /// </summary>
        /// <typeparam name="K">The type of the key list in the directory to retrieve</typeparam>
        /// <typeparam name="V">The type of the value list in the directory to retrieve</typeparam>
        /// <param name="key">The key from which to retrieve the directory</param>
        /// <returns>The directory of data, this will be empty if no data is found.</returns>
        public Dictionary<K, V> GetDictionary<K, V>(string key) {
            // Get the child node the dictionary is saved in.
            FileNode ChildNode = GetOrCreateNode(key);

            // Get the Keys list and values list.
            List<K> Keys = ChildNode.GetList<K>("Keys");
            List<V> Values = ChildNode.GetList<V>("Values");

            // Pair the indexes of the Keys and Values list to create the directory.
            Dictionary<K, V> Dict = Keys.Zip(Values, (k, v) => new { Key = k, Value = v })
                     .ToDictionary(x => x.Key, x => x.Value);

            // Return the directory.
            return Dict;
        }

        /// <summary>
        /// Attempts to retrieve the value at the supplied location.
        /// </summary>
        /// <typeparam name="T">The type of the value to retrieve.</typeparam>
        /// <param name="key">The key to retrieve the value from.</param>
        /// <param name="defaultValue">The value to return if the value could not be retrieved.</param>
        /// <returns>The value if found, else  the default value returned.</returns>
        public T GetValueOrElse<T>(string key, T defaultValue) {
            // Check if the key exists within this nodes data. If it doesen't return the default value.
            if (!nodeData.ContainsKey(key)) {
                return defaultValue;
            }

            // Attempt to get the Object stored at the keys location.
            Object value;
            nodeData.TryGetValue(key, out value);

            // If the object is of a serializable type we want to get its deserialized form to return
            // Request the serializer for the type and attempt to deserialize the Object.
            if (SerializerService.GetSerializer<T>() != null) {
                Serializable<T> serializer = SerializerService.GetSerializer<T>();
                return serializer.Deserialize((FileNode)Convert.ChangeType(value, typeof(FileNode)));
            }

            // Check if the retrieved value is of the type T. If it isn't throw an exception
            try {
                // Enums need special treatment... still checks if type is of T but using methods explicitly
                // defined for Enums.
                T converted = (T)Convert.ChangeType(value, typeof(T));
            } catch (Exception ex) {
                return defaultValue;
            }

            // Return the requested value.
            if (typeof(T).IsEnum) {
                return (T)Enum.Parse(typeof(T), (string)value, true);
            } else {
                return (T)Convert.ChangeType(value, typeof(T));
            }
        }

        /// <summary>
        /// Attempts to set the specified value at the specified location within this
        /// FileNode.
        /// </summary>
        /// <typeparam name="T">The type of the value to set.</typeparam>
        /// <param name="key">The location at which to store the value.</param>
        /// <param name="value">The value to store.</param>
        public void SetValue<T>(string key, T value) {
            Object valueToSet = value;

            // If the Type of the value has an assossiated serializer, use it to serialize the value.
            if (SerializerService.GetSerializer<T>() != null) {
                Serializable<T> serializer = SerializerService.GetSerializer<T>();
                valueToSet = serializer.Serialize(value);
            }

            // Check if the node already contains data at this location, if so overwrite any data 
            // stored at this location with the newly supplied value.
            if (nodeData.ContainsKey(key)) {
                if (typeof(T).IsEnum) {
                    nodeData[key] = (int)valueToSet;
                } else {
                    //Overwrite the value
                    nodeData[key] = valueToSet; 
                }
                return;
            }

            //Add the Key and Value to the FileNode's data.
            if (typeof(T).IsEnum) {
                nodeData.Add(key, (int)valueToSet); 
            } else {
                nodeData.Add(key, valueToSet);
            }
        }

        /// <summary>
        /// Attempts to set the specified list at the specified location within this
        /// FileNode
        /// </summary>
        /// <typeparam name="V">The type of the lists values</typeparam>
        /// <param name="key">The location at which to store this list.</param>
        /// <param name="values">The list of values to store.</param>
        public void SetList<V>(string key, List<V> values) {
            // Create a new child node and store each element of the list as a value with its key being the index,
            // and the value being the value.
            FileNode ListNode = GetOrCreateNode(key);
            for (int i = 0; i < values.Count; i++) {
                ListNode.SetValue(i.ToString(), values[i]);
            }
        }

        /// <summary>
        /// Attempts to set the specified directory at the specified location within this
        /// FileNode
        /// </summary>
        /// <typeparam name="K">The type of the keys in this dictionary.</typeparam>
        /// <typeparam name="V">the type of the values in this dictionary.</typeparam>
        /// <param name="key">The location at which to store this dictionary.</param>
        /// <param name="Dict">The dictionary to store,</param>
        public void SetDictionary<K, V>(string key, Dictionary<K, V> Dict) {
            // Create a new child node and store the dictionary as two lists inside it, a Keys list and a Values list.
            FileNode DictionaryNode = GetOrCreateNode(key);

            List<K> Keys = new List<K>(Dict.Keys);
            List<V> Values = new List<V>(Dict.Values);

            DictionaryNode.SetList<K>("Keys", Keys);
            DictionaryNode.SetList<V>("Values", Values);
        }

        /// <summary>
        /// Gets the raw node data stored within this FileNode.
        /// </summary>
        /// <returns>A Dictionary containing all of this nodes data.</returns>
        public Dictionary<string, Object> GetRawData() {
            return nodeData;
        }

        /// <summary>
        /// Gets this nodes data in a format which can be written to a file.
        /// </summary>
        /// <returns>An Array ordered by line which can be written to a file allowing
        /// for data to persist in a program</returns>
        public List<string> GetFileWriteableData() {
            List<string> fileData = new List<string>();

            // Iterate through the raw data
            foreach (KeyValuePair<string, Object> keyValuePair in nodeData) {
                if (keyValuePair.Value.GetType().Equals(typeof(FileNode))) {
                    // If the value attempting to get writable data for is a FileNode, write a Node start value, get
                    // its file writable data and then write a Node end value. This method will most likely be used
                    // recursively many times.
                    fileData.Add(keyValuePair.Key + "{");
                    fileData.AddRange(((FileNode)keyValuePair.Value).GetFileWriteableData());
                    fileData.Add("}");
                } else {
                    // Simply write the key then the value separated with an '=' character.
                    fileData.Add(keyValuePair.Key + "=" + keyValuePair.Value);
                }
            }

            return fileData;
        }
    }

    /// <summary>
    /// A class which groups any exceptions thrown by a FileNode.
    /// </summary>
    class NodeException {
        /// <summary>
        /// An exception which is thrown when a node value could not be
        /// found.
        /// </summary>
        public class NotFound : Exception { }

        /// <summary>
        /// An exception which is thrown when the located value is not of the
        /// requested type.
        /// </summary>
        public class IncorrectType : Exception { }

        /// <summary>
        /// An exception which is thrown when a type of object is attempted
        /// to be saved/loaded which has no assossiated serializer.
        /// </summary>
        public class NotSerializable : Exception { }

        /// <summary>
        /// An exception which is thrown when an object could not be deserialized
        /// because it depends on another object being deserialized first.
        /// </summary>
        public class FailedDependantDeserialization : Exception { }
    }
}
