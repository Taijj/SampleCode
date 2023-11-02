using System;
using System.Linq;
using System.Reflection;

public static class TypeAddons
{
	#region Types
	/// <summary>
	/// Looks through all assemblies and returns the first type found
	/// with the given name. Returns null, if none is found.
	/// NOTE: Uses Reflection!
	/// </summary>
	public static Type Find(string typeName)
    {
        Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();
        for (int i = 0; i < assemblies.Length; i++)
        {
            Type[] types = assemblies[i].GetTypes();
            for (int j = 0; j < types.Length; j++)
            {
                if (types[j].Name == typeName)
                    return types[j];
            }
        }

        return null;
    }

    /// <summary>
    /// Returns all Types in all assemblies that derive from the given Type.
    /// NOTE: Uses Reflection AND Linq!
    /// </summary>
    public static Type[] GetSubtypesOf(Type type)
    {
        return AppDomain.CurrentDomain.GetAssemblies()
            .SelectMany(a => a.GetTypes())
            .Where(t => type.IsAssignableFrom(t))
            .Where(t => t != type)
            .ToArray();
    }
	#endregion



	#region Enums
	public static bool IsEither(this Enum @this, params Enum[] values)
	{
		return values.Contains(@this);
	}

	public static bool IsNeither(this Enum @this, params Enum[] values)
	{
		return false == values.Contains(@this);
	}
	#endregion
}