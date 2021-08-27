public static class ExtensionMethods
{
    public static void CopyTo<T>(this T[] fromArray, T[] toArray)
    {
        float length = fromArray.Length > toArray.Length ? toArray.Length : fromArray.Length;
		
        for(int i = 0; i < length; i++)
            toArray[i] = fromArray[i];
    }
}