﻿using Sirenix.Serialization;
using UnityEngine;

#if UNITY_2017_2_OR_NEWER

//-----------------------------------------------------------------------
// <copyright file="VectorIntFormatters.cs" company="Sirenix IVS">
// Copyright (c) Sirenix IVS. All rights reserved.
// </copyright>
//-----------------------------------------------------------------------

[assembly: RegisterFormatter( typeof(Vector2IntFormatter) )]
[assembly: RegisterFormatter( typeof(Vector3IntFormatter) )]
namespace Sirenix.Serialization
{

    /// <summary>
    ///     Custom formatter for the <see cref="Vector2Int" /> type.
    /// </summary>
    /// <seealso cref="Sirenix.Serialization.MinimalBaseFormatter{UnityEngine.Vector2Int}" />
    public class Vector2IntFormatter : MinimalBaseFormatter<Vector2Int>
    {
        #region Constants and Statics

        private static readonly Serializer<int> Serializer = Serialization.Serializer.Get<int>();

        #endregion

        #region Protected Methods

        /// <summary>
        ///     Reads into the specified value using the specified reader.
        /// </summary>
        /// <param name="value">The value to read into.</param>
        /// <param name="reader">The reader to use.</param>
        protected override void Read( ref Vector2Int value, IDataReader reader )
        {
            value.x = Serializer.ReadValue( reader );
            value.y = Serializer.ReadValue( reader );
        }

        /// <summary>
        ///     Writes from the specified value using the specified writer.
        /// </summary>
        /// <param name="value">The value to write from.</param>
        /// <param name="writer">The writer to use.</param>
        protected override void Write( ref Vector2Int value, IDataWriter writer )
        {
            Serializer.WriteValue( value.x, writer );
            Serializer.WriteValue( value.y, writer );
        }

        #endregion
    }

    /// <summary>
    ///     Custom formatter for the <see cref="Vector3Int" /> type.
    /// </summary>
    /// <seealso cref="Sirenix.Serialization.MinimalBaseFormatter{UnityEngine.Vector3Int}" />
    public class Vector3IntFormatter : MinimalBaseFormatter<Vector3Int>
    {
        #region Constants and Statics

        private static readonly Serializer<int> Serializer = Serialization.Serializer.Get<int>();

        #endregion

        #region Protected Methods

        /// <summary>
        ///     Reads into the specified value using the specified reader.
        /// </summary>
        /// <param name="value">The value to read into.</param>
        /// <param name="reader">The reader to use.</param>
        protected override void Read( ref Vector3Int value, IDataReader reader )
        {
            value.x = Serializer.ReadValue( reader );
            value.y = Serializer.ReadValue( reader );
            value.z = Serializer.ReadValue( reader );
        }

        /// <summary>
        ///     Writes from the specified value using the specified writer.
        /// </summary>
        /// <param name="value">The value to write from.</param>
        /// <param name="writer">The writer to use.</param>
        protected override void Write( ref Vector3Int value, IDataWriter writer )
        {
            Serializer.WriteValue( value.x, writer );
            Serializer.WriteValue( value.y, writer );
            Serializer.WriteValue( value.z, writer );
        }

        #endregion
    }

}

#endif