using System.IO;

public interface IBinarySerializer<T>
{
    public void Serialize(T value, BinaryWriter writer);
}
