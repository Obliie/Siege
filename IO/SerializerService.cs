using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Siege.IO {
    public class SerializerService {
        private static Dictionary<Type, Serializable> serializers = new Dictionary<Type, Serializable>();

        /// <summary>
        /// Registers a serializer for a specific type.
        /// </summary>
        /// <param name="type">The type to register a serializer for.</param>
        /// <param name="serializer">The serializer to register.</param>
        public static void RegisterSerializer<T>(Serializable<T> serializer) {
            if (serializers.ContainsKey(typeof(T))) {
                // Serializer already registered, do nothing.
                return;
            }
            serializers.Add(typeof(T), serializer);
        }

        /// <summary>
        /// Gets the serializer for the specified type, it may be null if it hasnt been
        /// registered or doesent exist.
        /// </summary>
        /// <param name="type">The type to get a serializer for.</param>
        /// <returns>The serializer</returns>
        public static Serializable<T> GetSerializer<T>() {
            Serializable serializer;
            serializers.TryGetValue(typeof(T), out serializer);
            
            // Cast the serializer to its generic type, this is safe as non generic implementaions do NOT
            // exist.
            return (Serializable<T>) serializer;
        }
    }
}
