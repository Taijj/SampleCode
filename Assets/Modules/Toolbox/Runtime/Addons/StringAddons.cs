
using System;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

public static class StringAddons
{
    #region Common String Constants
    public const string LINEBREAK_DEFAULT = "\n";
    public const string LINEBREAK_TAG = "<br>";
    public const string TAB = "\t";
    public const string WHITE_SPACE = " ";
    public const string SLASH = "/";
    public const string BACKSLASH = "\\";
    public const string COMMA = ",";
    public const string DOT = ".";
	public const string SEMICOLON = ";";
	public const string UNDESCORE = "_";
    public const string EMPTY = ""; // Because string.Empty cannot be used as a method default parameter!

    public const string ASSETS = "Assets";
    public const string OBVIOUS_SEPARATOR_LINE = "==========================================================================";
    #endregion



    #region Methods
    private const string NUMERIC_PLACEHOLDER_REGEX = "{\\d+}";

    /// <summary>
    /// Returns true, if this string contains any numbered placeholder for
    /// text formatting ({0}, {1}, {n}), false otherwise.
    /// </summary>
    public static bool ContainsNumericPlaceholder(this string @this)
    {
        return Regex.IsMatch(@this, NUMERIC_PLACEHOLDER_REGEX);
    }

	/// <summary>
	/// Converts the string into CamelCase notation without whitespaces,
	/// and returns the new string.
	/// Taken from: https://stackoverflow.com/questions/18627112/how-can-i-convert-text-to-pascal-case
	/// </summary>
	public static string ToPascalCase(this string @this)
	{
		Regex invalidCharsRgx = new Regex("[^_a-zA-Z0-9]");
		Regex whiteSpace = new Regex(@"(?<=\s)");
		Regex startsWithLowerCaseChar = new Regex("^[a-z]");
		Regex firstCharFollowedByUpperCasesOnly = new Regex("(?<=[A-Z])[A-Z0-9]+$");
		Regex lowerCaseNextToNumber = new Regex("(?<=[0-9])[a-z]");
		Regex upperCaseInside = new Regex("(?<=[A-Z])[A-Z]+?((?=[A-Z][a-z])|(?=[0-9]))");

		// Replace white spaces with undescore, then replace all invalid characters with an empty string.
		var pascalCase = invalidCharsRgx.Replace(whiteSpace.Replace(@this, "_"), EMPTY)
			// Split by underscores
			.Split(new char[] { UNDESCORE[0] }, StringSplitOptions.RemoveEmptyEntries)
			// Set the first letter to uppercase
			.Select(w => startsWithLowerCaseChar.Replace(w, m => m.Value.ToUpper()))
			// Replace the second and all following upper case letters to lower, if there is no next lower (ABC -> Abc)
			.Select(w => firstCharFollowedByUpperCasesOnly.Replace(w, m => m.Value.ToLower()))
			// Set the first lower case following a number to uppser (Ab9cd -> Ab9Cd)
			.Select(w => lowerCaseNextToNumber.Replace(w, m => m.Value.ToUpper()))
			// Lower second and next upper case letters except the last, if it follows by any lower (ABcDEf -> AbcDef)
			.Select(w => upperCaseInside.Replace(w, m => m.Value.ToLower()));

		return string.Concat(pascalCase);
	}

	/// <summary>
	/// Converts line endings to the current OS line endings.
	/// </summary>
	public static string ToOsLineEndings(this string @this)
    {
        string endings = Application.platform != RuntimePlatform.WindowsEditor ? "\n" : "\r\n";
        return Regex.Replace(@this, "\\r\\n?|\\n", endings);
    }



    /// <summary>
    /// Converts a Unity Assets path to a system OS path.
    /// </summary>
    public static string ToSystemPath(this string @this)
    {
		return Path.Join(Application.dataPath.Replace(ASSETS, EMPTY), @this);
    }

    /// <summary>
    /// Converts a system OS path to a Unity Assets path.
    /// </summary>
    public static string ToAssetPath(this string @this)
    {
        int index = @this.IndexOf(ASSETS);
        return @this.Remove(0, index);
    }

    /// <summary>
    /// Wraps the string in quotations marks, and returns the new string.
    ///
    /// Some Terminals, like the windows command line, can only handle whitespaces,
    /// if a path is wrapped in quotation marks.
    /// </summary>
    public static string ToTerminalPath(this string @this)
    {
        return $"\"{@this}\"";
    }
    #endregion
}