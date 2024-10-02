using System.Collections.Generic;

public static class ListExtensions
{
    public static void Expand<T>(this List<T> list, int newCount) where T : class
    {
        //то же самое можно сделать через AddRange(), что более оптимизированно. Так что видимо это переизобретание велосипеда.
        while (list.Count < newCount)
            list.Add(null);
    }
    
    /// <summary>
    /// Расширяет list до index+1 элементов и вставляет value по index
    /// </summary>
    public static void AddAtIndexWithExpand<T>(this List<T> list, T value, int index) where T : class
    {
        if (list.Count < index + 1)
            list.Expand(index + 1);//TODO:заменить на AddRange()

        if (list[index] != null)
            throw new System.Exception("Значение в ячейке уже занято");

        list[index] = value;
    }
}
