using System;
using System.Diagnostics.CodeAnalysis;

namespace SourceGenerator;

public interface IMapper
{
    bool CanMap(Type sourceType, Type targetType);
        
    [return: NotNullIfNotNull("source")]
    object Convert([System.Diagnostics.CodeAnalysis.AllowNull]object source);
        
    void Map(object source, object target);
}