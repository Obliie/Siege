using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Siege.Core {

    /// <summary>
    /// A class used to construct Objects with varying properties.
    /// </summary>
    public interface IBuilder<T> {

        /// <summary>
        /// Validates the data provided to the Builder.
        /// </summary>
        /// <returns>The Builder but if data is missing a BuilderExceptions.MissingData exception will be thrown.</returns>
        IBuilder<T> Validate();

        /// <summary>
        /// Constructs the Object based on the data provided to the Builder.
        /// </summary>
        /// <returns>The newly constructed Object T.</returns>
        T Build();
    }

    /// <summary>
    /// A class which groups any exceptions thrown by Builders.
    /// </summary>
    public class BuilderExceptions {
        /// <summary>
        /// An exception which is thrown when a Builder is not provided all the data it is
        /// required in order to build an Object.
        /// </summary>
        public class MissingData : Exception { }
    }
}
