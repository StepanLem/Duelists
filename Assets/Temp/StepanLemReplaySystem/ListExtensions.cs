using System.Collections.Generic;

public static class ListExtensions
{    
    /// <summary>
    /// Расширяет list до index+1 элементов и вставляет value по index
    /// </summary>
    public static void AddAtIndexWithExpand<T>(this List<T> list, T value, int index) where T : class
    {
        if (index > list.Count - 1)
            list.AddRange(new T[list.Count - index+1]);

        if (list[index] != null)
            throw new System.Exception("Значение в ячейке уже занято");

        list[index] = value;
    }
}
