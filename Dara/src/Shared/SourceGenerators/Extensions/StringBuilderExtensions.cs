using System;
using System.Text;

namespace Dara.Shared.SourceGenerators.Extensions;

public static class StringBuilderExtensions
{
    public static StringBuilder AppendIntendOnNewLine(this StringBuilder builder, int intend, bool ignoreLastNewLine = true)
    {
        if(builder.Length < 1)
            return builder;
        
        builder.Replace(
            "\n",
            "\n"+new string(' ', intend*4),
            0,
            ignoreLastNewLine ? builder.Length-1 : builder.Length
            );
        return builder;
    }
    
    //debug
    public static StringBuilder NewLineAsComment(this StringBuilder builder)
    {
        builder.Replace(
            "\n",
            "\n// "
            );
        return builder;
    }

    public static StringBuilder RemoveLastNewLine(this StringBuilder builder)
    {
        if(builder.Length < 1)
            return builder;
        
        var index = builder.Length - 1;
        while (index >= 0)
        {
            if (builder[index] == '\n')
                return builder.Remove(index, 1);
            index--;
        }
        
        return builder;
    }

    public static string FirstCharToLower(this string str)
    {
        if (string.IsNullOrEmpty(str))
            return String.Empty;
        
        if(str.Length < 2)
            return str;
        
        return char.ToLower(str[0]) + str.Substring(1);
    }
}