using System;

namespace NamelessGames.ExtensionMethods
{
    public static class ArrayExt
    {
        public static T[] Remove<T>(this T[] array, T value)
        {
            int found = 0;
            for (int index = 0; index < array.Length; index++)
            {
                if (array[index].Equals(value)) found++;
                if (index + found >= array.Length) break;
                array[index] = array[index + found];
            }
            Array.Resize(ref array, array.Length - found);
            return array;
        }
    }
}
