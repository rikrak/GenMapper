using System;

namespace SourceGenerator
{
    [AttributeUsage(AttributeTargets.Class, Inherited = false, AllowMultiple = false)]
    public sealed class MapperAttribute : Attribute
    {
        public Type SourceType { get; }
        public Type TargetType { get; }

        public MapperAttribute(Type sourceType, Type targetType)
        {
            SourceType = sourceType;
            TargetType = targetType;
        }
    }
}