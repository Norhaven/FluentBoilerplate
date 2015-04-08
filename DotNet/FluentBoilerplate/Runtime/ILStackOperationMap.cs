/*
   Copyright 2015 Chris Hannon

   Licensed under the Apache License, Version 2.0 (the "License");
   you may not use this file except in compliance with the License.
   You may obtain a copy of the License at

     http://www.apache.org/licenses/LICENSE-2.0

   Unless required by applicable law or agreed to in writing, software
   distributed under the License is distributed on an "AS IS" BASIS,
   WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
   See the License for the specific language governing permissions and
   limitations under the License.
 */

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading.Tasks;

namespace FluentBoilerplate.Runtime
{
    internal sealed class ILStackOperationMap
    {
        private sealed class Map
        {
        public OpCode OpCode { get; private set; }
        public int PushCount { get; private set; }
        public int PopCount { get; private set; }

        public Map(OpCode opcode, int pushCount, int popCount)
        {
            this.OpCode = opcode;
            this.PushCount = pushCount;
            this.PopCount = popCount;
        }
        }

        private static IDictionary<OpCode, Map> map = new Dictionary<OpCode, Map>
        {
            { OpCodes.Ldtoken, new Map(OpCodes.Ldtoken, 1, 0) },
            { OpCodes.Ldc_I4, new Map(OpCodes.Ldc_I4, 1, 0) },
            
        };
    }
}
