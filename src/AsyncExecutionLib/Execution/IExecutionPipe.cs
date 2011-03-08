#region [ File Header ]
/*****************************************************************************
  Copyright 2011 Stefan Domnanovits

  Licensed under the Apache License, Version 2.0 (the "License");
  you may not use this file except in compliance with the License.
  You may obtain a copy of the License at
 
      http://www.apache.org/licenses/LICENSE-2.0
 
  Unless required by applicable law or agreed to in writing, software
  distributed under the License is distributed on an "AS IS" BASIS,
  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
  See the License for the specific language governing permissions and
  limitations under the License.
/****************************************************************************/
#endregion

namespace OnyxOx.AsyncExecutionLib.Execution
{
   using System;

   /// <summary>
   /// Executions jobs that have been added.
   /// </summary>
   public interface IExecutionPipe : IDisposable
   {
      /// <summary>
      /// Adds a job into the pipe to be executed.
      /// </summary>
      /// <param name="job">Job to exeucte.</param>
      void Add(IJob job);
   }
}
