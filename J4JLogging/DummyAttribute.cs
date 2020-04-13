using System;

namespace J4JSoftware.Logging
{
    // added to test SharpDoc
    [AttributeUsage( validOn: AttributeTargets.Class | AttributeTargets.Interface, Inherited = false, AllowMultiple = true )]
    public class DummyAttribute : Attribute
    {
        public event EventHandler<int> Ralph;
        
        public DummyAttribute( string arg1, Type arg2 )
        {
        }

        public int TestField;
    }

    public interface IDummyInterface1
    {
        int Number { get; set; }
    }

    public interface IDummyInterface2 : IDummyInterface1
    {
        string Text { get; set; }
    }

    public interface IDummyInterface3<in T>
        where T : DummyAttribute
    {
        string GetValue( T item );

        bool TestGenericMethod<TMethod>()
            where TMethod : class, IDummyInterface1;
    }
}
