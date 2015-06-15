using System;
using System.Runtime.Serialization;

namespace Mentula.Content
{
    public class ParameterNullException : Exception
    {
        public string Parameter;

        public ParameterNullException()
        {
            Parameter = "NULL";
        }

        public ParameterNullException(string parameter)
            : base(SetMessage(parameter))
        {
            Parameter = parameter;
        }

        public ParameterNullException(string parameter, Exception inner)
            : base(SetMessage(parameter), inner)
        {
            Parameter = parameter;
        }

        private static string SetMessage(string arg)
        {
            return string.Format("Could not find parameter: '{0}'.", arg);
        }
    }

    public class ParameterException : Exception
    {
        public Type Type;
        public string Parameter;
        public string Value;

        public ParameterException()
        {
            Type = typeof(void);
            Parameter = Value = "NULL";
        }

        public ParameterException(string parameter, string value, Type type)
            : base(SetMessage(parameter, value, type))
        {
            Type = type;
            Parameter = parameter;
            Value = value;
        }

        public ParameterException(string parameter, string value, Type type, Exception inner)
            : base(SetMessage(parameter, value, type), inner)
        {
            Type = type;
            Parameter = parameter;
            Value = value;
        }

        private static string SetMessage(string parameter, string value, Type type)
        {
            return string.Format("Could not process parameter '{0}' to {1}. Value='{2}'", parameter, type, value);
        }
    }
}