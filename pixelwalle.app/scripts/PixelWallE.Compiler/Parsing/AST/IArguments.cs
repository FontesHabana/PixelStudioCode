namespace PixelWallE.Language.Parsing;
using PixelWallE.Language.Parsing.Expressions;

using System.Collections.Generic;
public interface IArgument<T>{
     
     List<T> Args{get;set;}

}