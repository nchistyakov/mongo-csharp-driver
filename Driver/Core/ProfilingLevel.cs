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

namespace MongoDB.Driver
{
    /// <summary>
    /// Represents what level of profile information to write.
    /// </summary>
    public enum ProfilingLevel
    {
        /// <summary>
        /// Don't write profile information for any queries.
        /// </summary>
        None = 0,
        /// <summary>
        /// Write profile information for slow queries.
        /// </summary>
        Slow = 1,
        /// <summary>
        /// Write profile information for all queries.
        /// </summary>
        All = 2
    }
}
