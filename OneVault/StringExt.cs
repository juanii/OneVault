﻿using System;

static internal class StringExt
{
    public static string FixNewLines(string str)
    {
        return ReplaceNewLines(str, Environment.NewLine);
    }

    public static string ReplaceNewLines(string str, string rpl)
    {
        if (string.IsNullOrEmpty(str))
            return str;

        return str.Replace("\r\n", "\n").Replace("\r", "\n").Replace("\n", rpl);
    }

    public static string Reverse(string str)
    {
        char[] charArray = str.ToCharArray();
        Array.Reverse(charArray);
        return new string(charArray);
    }
}