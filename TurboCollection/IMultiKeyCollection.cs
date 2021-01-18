using System;
using System.Collections.Generic;

namespace TurboCollection
{
    public interface IMultiKeyCollection<TKeyId, TKeyName, TValue> :
        IDictionary<CompositeKey<TKeyId, TKeyName>, TValue>
        where TKeyId : notnull
        where TKeyName : notnull
    {
        /// <summary>
        /// Gets the values associated with the specified key by part <see cref="TKeyId"/>.
        /// </summary>
        /// <exception cref="ArgumentNullException"> key is null. </exception>
        /// <exception cref="KeyNotFoundException"> The property is retrieved and key does not exist in the collection. </exception>
        IEnumerable<TValue> GetValueById(TKeyId keyId);

        /// <summary>
        /// Gets the values associated with the specified key by part <see cref="TKeyName"/>.
        /// </summary>
        /// <exception cref="ArgumentNullException"> key is null. </exception>
        /// <exception cref="KeyNotFoundException"> The property is retrieved and key does not exist in the collection. </exception>
        IEnumerable<TValue> GetValueByName(TKeyName keyName);
    }
}