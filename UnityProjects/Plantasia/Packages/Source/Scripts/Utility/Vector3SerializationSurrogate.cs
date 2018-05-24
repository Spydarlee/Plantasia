using UnityEngine;
using System.Runtime.Serialization;

/// <summary>
/// The .net BinaryFormatter can't serialise Vector3 objects out of the box.
/// This surrogate gets around that by (de)serialising as three floats instead.
/// </summary>
public class Vector3SerializationSurrogate : ISerializationSurrogate
{
    // -------------------------------------------------------------------------------

    public void GetObjectData(System.Object obj, SerializationInfo info, StreamingContext context)
    {
        Vector3 vector3Object = (Vector3)obj;
        info.AddValue("x", vector3Object.x);
        info.AddValue("y", vector3Object.y);
        info.AddValue("z", vector3Object.z);
    }

    // -------------------------------------------------------------------------------

    public System.Object SetObjectData(System.Object obj, SerializationInfo info, StreamingContext context, ISurrogateSelector selector)
    {
        Vector3 vector3Object = (Vector3)obj;
        vector3Object.x = (float)info.GetValue("x", typeof(float));
        vector3Object.y = (float)info.GetValue("y", typeof(float));
        vector3Object.z = (float)info.GetValue("z", typeof(float));

        obj = vector3Object;
        return obj;
    }

    // -------------------------------------------------------------------------------
}
