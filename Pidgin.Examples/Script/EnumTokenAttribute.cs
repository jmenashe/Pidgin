using System;
using System.Collections.Generic;
using System.Text;
using System.ComponentModel;

namespace Pidgin.Examples.Script
{
    [AttributeUsage(AttributeTargets.Field)]
    public class EnumTokenAttribute : Attribute
    {
        public string Token { get; private set; }
        public EnumTokenAttribute(int value, string token)
        {
            this.Token = token;
        }
    }
}
