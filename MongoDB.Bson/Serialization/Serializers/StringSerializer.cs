﻿/* Copyright 2010-2013 10gen Inc.
*
* Licensed under the Apache License, Version 2.0 (the "License");
* you may not use this file except in compliance with the License.
* You may obtain a copy of the License at
*
* http://www.apache.org/licenses/LICENSE-2.0
*
* Unless required by applicable law or agreed to in writing, software
* distributed under the License is distributed on an "AS IS" BASIS,
* WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
* See the License for the specific language governing permissions and
* limitations under the License.
*/

using System;
using System.IO;
using MongoDB.Bson.IO;
using MongoDB.Bson.Serialization.Options;

namespace MongoDB.Bson.Serialization.Serializers
{
    /// <summary>
    /// Represents a serializer for Strings.
    /// </summary>
    public class StringSerializer : BsonBaseSerializer
    {
        // private static fields
        private static StringSerializer __instance = new StringSerializer();

        // constructors
        /// <summary>
        /// Initializes a new instance of the StringSerializer class.
        /// </summary>
        public StringSerializer()
            : base(new RepresentationSerializationOptions(BsonType.String))
        {
        }

        // public static properties
        /// <summary>
        /// Gets an instance of the StringSerializer class.
        /// </summary>
        [Obsolete("Use constructor instead.")]
        public static StringSerializer Instance
        {
            get { return __instance; }
        }

        // public methods
        /// <summary>
        /// Deserializes an object from a BsonReader.
        /// </summary>
        /// <param name="bsonReader">The BsonReader.</param>
        /// <param name="nominalType">The nominal type of the object.</param>
        /// <param name="actualType">The actual type of the object.</param>
        /// <param name="options">The serialization options.</param>
        /// <returns>An object.</returns>
        public override object Deserialize(
            BsonReader bsonReader,
            Type nominalType,
            Type actualType,
            IBsonSerializationOptions options)
        {
            VerifyTypes(nominalType, actualType, typeof(string));
            var representationSerializationOptions = EnsureSerializationOptions<RepresentationSerializationOptions>(options);

            var bsonType = bsonReader.GetCurrentBsonType();
            if (bsonType == BsonType.Null)
            {
                bsonReader.ReadNull();
                return null;
            }
            else
            {
                switch (bsonType)
                {
                    case BsonType.ObjectId:
                        if (representationSerializationOptions.Representation == BsonType.ObjectId)
                        {
                            return bsonReader.ReadObjectId().ToString();
                        }
                        else
                        {
                            goto default;
                        }
                    case BsonType.String:
                        return bsonReader.ReadString();
                    case BsonType.Symbol:
                        return bsonReader.ReadSymbol();
                    default:
                        var message = string.Format("Cannot deserialize string from BsonType {0}.", bsonType);
                        throw new FileFormatException(message);
                }
            }
        }

        /// <summary>
        /// Serializes an object to a BsonWriter.
        /// </summary>
        /// <param name="bsonWriter">The BsonWriter.</param>
        /// <param name="nominalType">The nominal type.</param>
        /// <param name="value">The object.</param>
        /// <param name="options">The serialization options.</param>
        public override void Serialize(
            BsonWriter bsonWriter,
            Type nominalType,
            object value,
            IBsonSerializationOptions options)
        {
            if (value == null)
            {
                bsonWriter.WriteNull();
            }
            else
            {
                var stringValue = (string)value;
                var representationSerializationOptions = EnsureSerializationOptions<RepresentationSerializationOptions>(options);

                switch (representationSerializationOptions.Representation)
                {
                    case BsonType.ObjectId:
                        bsonWriter.WriteObjectId(ObjectId.Parse(stringValue));
                        break;
                    case BsonType.String:
                        bsonWriter.WriteString(stringValue);
                        break;
                    case BsonType.Symbol:
                        bsonWriter.WriteSymbol(stringValue);
                        break;
                    default:
                        var message = string.Format("'{0}' is not a valid String representation.", representationSerializationOptions.Representation);
                        throw new BsonSerializationException(message);
                }
            }
        }
    }
}
