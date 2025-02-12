namespace TaxCalculator.Infrastructure.Services;

public static class DictionaryExtensions {
    /// <summary>
    /// Gets the value associated with the specified key, or adds a new value if the key does not exist.
    /// </summary>
    /// <typeparam name="TKey">The type of the key.</typeparam>
    /// <typeparam name="TValue">The type of the value.</typeparam>
    /// <param name="dictionary">The dictionary to operate on.</param>
    /// <param name="key">The key to locate.</param>
    /// <param name="valueFactory">The factory function to create a value if the key does not exist.</param>
    /// <returns>The value associated with the key.</returns>
    public static TValue GetOrAdd<TKey, TValue>(
        this Dictionary<TKey, TValue> dictionary,
        TKey key,
        Func<TKey, TValue> valueFactory) {
        if (!dictionary.TryGetValue(key, out TValue? value)) {
            value = valueFactory(key);
            dictionary[key] = value;
        }

        return value;
    }
}

