﻿/* Copyright 2010-2012 10gen Inc.
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
using System.Collections.Generic;
using System.Linq;
using System.Text;

using MongoDB.Bson;
using MongoDB.Bson.Serialization;

namespace MongoDB.Driver
{
    /// <summary>
    /// Represents the result of a map/reduce command.
    /// </summary>
    public class MapReduceResult : CommandResult
    {
        // private fields
        private MongoDatabase _inputDatabase;

        // constructors
        /// <summary>
        /// Initializes a new instance of the MapReduceResult class.
        /// </summary>
        public MapReduceResult()
        {
        }

        // public properties
        /// <summary>
        /// Gets the output collection name (null if none).
        /// </summary>
        public string CollectionName
        {
            get
            {
                var result = _response["result", null];
                if (result != null)
                {
                    if (result.IsString)
                    {
                        return result.AsString;
                    }
                    else
                    {
                        return (string)result.AsBsonDocument["collection", null];
                    }
                }
                return null;
            }
        }

        /// <summary>
        /// Gets the output database name (null if none).
        /// </summary>
        public string DatabaseName
        {
            get
            {
                var result = _response["result", null];
                if (result != null && result.IsBsonDocument)
                {
                    return (string)result.AsBsonDocument["db", null];
                }
                return null;
            }
        }

        /// <summary>
        /// Gets the duration.
        /// </summary>
        public TimeSpan Duration
        {
            get { return TimeSpan.FromMilliseconds(_response["timeMillis"].ToInt32()); }
        }

        /// <summary>
        /// Gets the emit count.
        /// </summary>
        public long EmitCount
        {
            get { return _response["counts"].AsBsonDocument["emit"].ToInt64(); }
        }

        /// <summary>
        /// Gets the output count.
        /// </summary>
        public long OutputCount
        {
            get { return _response["counts"].AsBsonDocument["output"].ToInt64(); }
        }

        /// <summary>
        /// Gets the inline results.
        /// </summary>
        public IEnumerable<BsonDocument> InlineResults
        {
            get { return _response["results"].AsBsonArray.Cast<BsonDocument>(); }
        }

        /// <summary>
        /// Gets the input count.
        /// </summary>
        public long InputCount
        {
            get { return _response["counts"].AsBsonDocument["input"].ToInt64(); }
        }

        // public methods
        /// <summary>
        /// Gets the inline results as TDocuments.
        /// </summary>
        /// <typeparam name="TDocument">The type of the documents.</typeparam>
        /// <returns>The documents.</returns>
        public IEnumerable<TDocument> GetInlineResultsAs<TDocument>()
        {
            return GetInlineResultsAs(typeof(TDocument)).Cast<TDocument>();
        }

        /// <summary>
        /// Gets the inline results as TDocuments.
        /// </summary>
        /// <param name="documentType">The type of the documents.</param>
        /// <returns>The documents.</returns>
        public IEnumerable<object> GetInlineResultsAs(Type documentType)
        {
            return InlineResults.Select(document => BsonSerializer.Deserialize(document, documentType));
        }

        /// <summary>
        /// Gets the results (either inline or fetched from the output collection).
        /// </summary>
        /// <returns>The documents.</returns>
        public IEnumerable<BsonDocument> GetResults()
        {
            if (_response.Contains("results"))
            {
                return InlineResults;
            }
            else
            {
                var outputDatabaseName = DatabaseName;
                MongoDatabase outputDatabase;
                if (outputDatabaseName == null)
                {
                    outputDatabase = _inputDatabase;
                }
                else
                {
                    outputDatabase = _inputDatabase.Server[outputDatabaseName];
                }
                return outputDatabase[CollectionName].FindAll();
            }
        }

        /// <summary>
        /// Gets the results as TDocuments (either inline or fetched from the output collection).
        /// </summary>
        /// <typeparam name="TDocument">The type of the documents.</typeparam>
        /// <returns>The documents.</returns>
        public IEnumerable<TDocument> GetResultsAs<TDocument>()
        {
            return GetResultsAs(typeof(TDocument)).Cast<TDocument>();
        }

        /// <summary>
        /// Gets the results as TDocuments (either inline or fetched from the output collection).
        /// </summary>
        /// <param name="documentType">The type of the documents.</param>
        /// <returns>The documents.</returns>
        public IEnumerable<object> GetResultsAs(Type documentType)
        {
            if (_response.Contains("results"))
            {
                return GetInlineResultsAs(documentType);
            }
            else
            {
                var outputDatabaseName = DatabaseName;
                MongoDatabase outputDatabase;
                if (outputDatabaseName == null)
                {
                    outputDatabase = _inputDatabase;
                }
                else
                {
                    outputDatabase = _inputDatabase.Server[outputDatabaseName];
                }
                return outputDatabase[CollectionName].FindAllAs(documentType).Cast<object>();
            }
        }

        // internal methods
        internal void SetInputDatabase(MongoDatabase inputDatabase)
        {
            _inputDatabase = inputDatabase;
        }
    }
}
