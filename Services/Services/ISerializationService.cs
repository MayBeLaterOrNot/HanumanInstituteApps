﻿using System.IO;
using System.Text.Json.Serialization.Metadata;

namespace HanumanInstitute.Services;

/// <summary>
/// Manages the serialization of objects.
/// </summary>
public interface ISerializationService
{
    /// <summary>
    /// Serializes an object of specified type to a string.
    /// </summary>
    /// <typeparam name="T">The data type of the object to serialize.</typeparam>
    /// <param name="dataToSerialize">The object to serialize.</param>
    /// <param name="serializerContext">Optional, a serializer context generated by Json Source Generator.</param>
    /// <returns>An XML string containing serialized data.</returns>
    string Serialize<T>(T dataToSerialize, IJsonTypeInfoResolver? serializerContext);

    /// <summary>
    /// Serializes an object of specified type to a stream.
    /// </summary>
    /// <typeparam name="T">The data type of the object to serialize.</typeparam>
    /// <param name="utf8Json">A stream to write the serialized data to.</param>
    /// <param name="dataToSerialize">The object to serialize.</param>
    /// <param name="serializerContext">Optional, a serializer context generated by Json Source Generator.</param>
    Task SerializeAsync<T>(Stream utf8Json, T dataToSerialize, IJsonTypeInfoResolver? serializerContext);
    
    /// <summary>
    /// Deserializes an object of specified type from a string.
    /// </summary>
    /// <typeparam name="T">The data type of the object to deserialize.</typeparam>
    /// <param name="data">The XML string containing the data to deserialize.</param>
    /// <param name="serializerContext">Optional, a serializer context generated by Json Source Generator.</param>
    /// <exception cref="InvalidOperationException">An error occurred during deserialization. The original exception is available using the InnerException property.</exception>
    /// <returns>The deserialized object.</returns>
    T Deserialize<T>(string data, IJsonTypeInfoResolver? serializerContext) where T : class, new();

    /// <summary>
    /// Deserializes an object of specified type from a stream.
    /// </summary>
    /// <typeparam name="T">The data type of the object to deserialize.</typeparam>
    /// <param name="utf8Json">The data to deserialize.</param>
    /// <param name="serializerContext">Optional, a serializer context generated by Json Source Generator.</param>
    /// <exception cref="InvalidOperationException">An error occurred during deserialization. The original exception is available using the InnerException property.</exception>
    /// <returns>The deserialized object.</returns>
    ValueTask<T> DeserializeAsync<T>(Stream utf8Json, IJsonTypeInfoResolver? serializerContext)
        where T : class, new();
    
    /// <summary>
    /// Saves an object to a Json file.
    /// </summary>
    /// <typeparam name="T">The data type of the object to serialize.</typeparam>
    /// <param name="dataToSerialize">The object to serialize.</param>
    /// <param name="path">The path of the file in which to output the XML data.</param>
    /// <param name="serializerContext">Optional, a serializer context generated by Json Source Generator.</param>
    void SerializeToFile<T>(T dataToSerialize, string path, IJsonTypeInfoResolver? serializerContext);

    /// <summary>
    /// Saves an object to a Json file.
    /// </summary>
    /// <typeparam name="T">The data type of the object to serialize.</typeparam>
    /// <param name="dataToSerialize">The object to serialize.</param>
    /// <param name="path">The path of the file in which to output the XML data.</param>
    /// <param name="serializerContext">Optional, a serializer context generated by Json Source Generator.</param>
    Task SerializeToFileAsync<T>(T dataToSerialize, string path, IJsonTypeInfoResolver? serializerContext);
    
    /// <summary>
    /// Loads an object of specified type from a Json file.
    /// </summary>
    /// <typeparam name="T">The data type of the object to serialize.</typeparam>
    /// <param name="path">The path of the file from which to read XML data.</param>
    /// <param name="serializerContext">Optional, a serializer context generated by Json Source Generator.</param>
    /// <exception cref="InvalidOperationException">An error occurred during deserialization. The original exception is available using the InnerException property.</exception>
    /// <returns>The object created from the file</returns>
    T DeserializeFromFile<T>(string path, IJsonTypeInfoResolver? serializerContext);

    /// <summary>
    /// Loads an object of specified type from a Json file.
    /// </summary>
    /// <typeparam name="T">The data type of the object to serialize.</typeparam>
    /// <param name="path">The path of the file from which to read XML data.</param>
    /// <param name="serializerContext">Optional, a serializer context generated by Json Source Generator.</param>
    /// <exception cref="InvalidOperationException">An error occurred during deserialization. The original exception is available using the InnerException property.</exception>
    /// <returns>The object created from the file</returns>
    ValueTask<T> DeserializeFromFileAsync<T>(string path, IJsonTypeInfoResolver? serializerContext);
}
