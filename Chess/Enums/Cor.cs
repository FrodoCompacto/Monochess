using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Text;

namespace Chess.Enums
{
    public enum Cor
    {
        [Description("_A")]
        Branca,
        [Description("_B")]
        Preta,
        [Description("_C")]
        Azul,
}

    public static class CorSuffix
    {
        public static string Suffix(this Cor val)
        {
            DescriptionAttribute[] attributes = (DescriptionAttribute[])val
               .GetType()
               .GetField(val.ToString())
               .GetCustomAttributes(typeof(DescriptionAttribute), false);
            return attributes.Length > 0 ? attributes[0].Description : string.Empty;
        }
    }

}
