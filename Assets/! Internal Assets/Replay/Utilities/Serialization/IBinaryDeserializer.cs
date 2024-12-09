using System.IO;

public interface IBinaryDeserializer<T>
{
    public T Deserialize(BinaryReader reader);
}
