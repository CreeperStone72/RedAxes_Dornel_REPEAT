using System;
using System.IO;
using System.Runtime.Serialization;
using System.Xml;
using UnityEngine;

namespace GD.Utilities
{

    /// <summary>
    ///     Provides XML serialization functionality for any class which implements a DataContract
    ///     NOTE: These methods DO NOT test (e.g. using Reflection) that the objects to be
    ///     serialized conform to the DataContract (i.e. include DataContract, DataMember attributes)
    /// </summary>
    /// <see cref="DemoSerializationTransform" />
    /// <seealso cref="https://docs.microsoft.com/en-us/dotnet/framework/wcf/samples/datacontractserializer-sample" />
    public sealed class SerializationUtility
    {
        #region Public Methods

        public static object Load( string name, Type type )
        {
            var fStream = new FileStream( name, FileMode.Open );
            var textReader = XmlDictionaryReader.CreateTextReader( fStream, new XmlDictionaryReaderQuotas() );
            var objSerializer =
                new DataContractSerializer( type ); //TODO - add check on Type to ensure its serializable

            object deserializedObject = objSerializer.ReadObject( textReader, true );
            textReader.Close();
            fStream.Close();
            return deserializedObject;
        }

        public static void Save( string name, object obj )
        {
            //"/Data/NPC/characteristics.xml"

            var dataContractSerializer =
                new DataContractSerializer( obj.GetType() ); //TODO - add check on Type to ensure its serializable
            var xmlSettings = new XmlWriterSettings();
            xmlSettings.Indent = true;
            xmlSettings.IndentChars = "\t";
            var xmlWriter = XmlWriter.Create( name, xmlSettings );

            dataContractSerializer.WriteObject( xmlWriter, obj );
            xmlWriter.Flush(); //flushes any objects in the outputstream to target/disk/network - as in life, always flush
            xmlWriter.Close();
        }

        #endregion
    }

    [DataContract]
    public class DemoSerializationTransform
    {
        #region Private Fields

        private string password;

        #endregion

        #region Constructors

        public DemoSerializationTransform()
            : this( Vector3.zero, Vector3.zero, Vector3.one ) { }

        public DemoSerializationTransform( Vector3 translation, Vector3 rotation, Vector3 scale )
        {
            LocalTranslation = translation;
            LocalRotation = rotation;
            LocalScale = scale;
        }

        #endregion

        #region Properties

        [DataMember]
        public Vector3 LocalTranslation { get; set; }

        [DataMember]
        public Vector3 LocalRotation { get; set; }

        [DataMember]
        public Vector3 LocalScale { get; set; }

        #endregion
    }

}