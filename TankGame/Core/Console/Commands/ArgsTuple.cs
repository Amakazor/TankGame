using System;
using LanguageExt;

namespace TankGame.Core.Console.Commands; 

public class ArgsTuple<T> : Tuple<Seq<string>, T> 
{
    public Seq<string> Args => Item1;
    public T Value => Item2;
    public ArgsTuple(Seq<string> item1, T item2) : base(item1, item2) { }
}
    