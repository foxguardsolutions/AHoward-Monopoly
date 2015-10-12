using System;

namespace Monopoly
{
    public interface IPropertyGroup
    {
        IProperty[] Properties { get; set; }
        string Name { get; set; }
        string RentFunction { get; set; }

        IProperty GetPropertyFromPropertyIndex(int index);
        IProperty GetPropertyFromName(string name);
    }
}
