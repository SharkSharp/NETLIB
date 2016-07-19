namespace NETLIB
{
    /// <summary>
    /// Interface which describes the methods for packing and unpacking the calling Class in a package
    /// called BasePack. 
    /// Use when you want to send/receive the class on the network.
    /// This interface describe the necessary methods that allow the class insert itself in a BasePack.
    /// Used by BasePack to insert a custom class in the buffer
    /// </summary>
    /// <seealso cref="BasePack.PutPackable{CustomType}(CustomType)"/>
    /// <seealso cref="BasePack.GetPackable{CustomType}"/>
    /// /// <example>
    /// This example shows how to properly implement the interface IPackable and its methods.
    /// <code>
    /// public class Test : IPackable
    /// {
    ///     private int i;
    ///     private float j;
    ///     private double k;
    ///     private string str;    
    /// 
    ///     public Pack(BasePack pack)
    ///     {
    ///         pack.PutInt(i);
    ///         pack.PutFloat(j);
    ///         pack.PutDouble(k);
    ///         pack.PutString(str);
    ///     }
    /// 
    ///     public Unpack(BasePack pack)
    ///     {
    ///         this.i = pack.GetInt();
    ///         this.j = pack.GetFloat();
    ///         this.k = pack.GetDouble();
    ///         this.str = pack.GetString();
    ///     }
    /// }
    /// </code>
    /// </example>
    public interface IPackable
    {
        /// <summary>
        /// The class must implement this method in order to fill a BasePack, inserting its fields
        /// and other desired information on the <paramref name="pack"/>. The fiels and information MUST BE in the same orther 
        /// than they will be unpacked.
        /// </summary>
        /// <param name="pack">Pack to put the fields and other desired information.</param>
        /// <seealso cref="BasePack.PutPackable{CustomType}(CustomType)"/>
        /// <example>
        /// This example shows how to put the class fields in the <paramref name="pack"/>.
        /// <code>
        /// public class Test : IPackable
        /// {
        ///     private int i;
        ///     private float j;
        ///     private double k;
        ///     private string str;    
        /// 
        ///     public Pack(BasePack pack)
        ///     {
        ///         pack.PutInt(i);
        ///         pack.PutFloat(j);
        ///         pack.PutDouble(k);
        ///         pack.PutString(str);
        ///     }
        /// 
        ///     public Unpack(BasePack pack)
        ///     {
        ///         this.i = pack.GetInt();
        ///         this.j = pack.GetFloat();
        ///         this.k = pack.GetDouble();
        ///         this.str = pack.GetString();
        ///     }
        /// }
        /// </code>
        /// </example>
        void Pack(BasePack pack);

        /// <summary>
        /// The class must implement this method in order to extract fields
        /// and other desired information from a BasePack. The fiels and information MUST BE in the same orther 
        /// than they were packed.
        /// The method <see cref="BasePack.GetPackable{CustomType}"/> use this method as a contructor, to get the necessary field 
        /// and information from the <paramref name="pack"/> after creating a new instance of the class that implements
        /// <see cref="IPackable"/>, with parameterless contructor. 
        /// </summary>
        /// <param name="pack">Pack that contains the fields and other informations.</param>
        /// <seealso cref="BasePack.GetPackable{CustomType}"/>
        /// <example>
        /// This exemple shows how to restore the class field from the <paramref name="pack"/> using the Unpack method.
        /// <code>
        /// public class Test : IPackable
        /// {
        ///     private int i;
        ///     private float j;
        ///     private double k;
        ///     private string str;    
        /// 
        ///     public Pack(BasePack pack)
        ///     {
        ///         pack.PutInt(i);
        ///         pack.PutFloat(j);
        ///         pack.PutDouble(k);
        ///         pack.PutString(str);
        ///     }
        /// 
        ///     public Unpack(BasePack pack)
        ///     {
        ///         this.i = pack.GetInt();
        ///         this.j = pack.GetFloat();
        ///         this.k = pack.GetDouble();
        ///         this.str = pack.GetString();
        ///     }
        /// }
        /// </code>
        /// </example>
        void Unpack(BasePack pack);
    }
}