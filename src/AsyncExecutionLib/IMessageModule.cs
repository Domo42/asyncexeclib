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

namespace OnyxOx.AsyncExecutionLib
{
   using System;

   /// <summary>
   /// Classes inheriting from this interface will be called before and after the
   /// message handling chain executes.
   /// </summary>
   public interface IMessageModule
   {
      /// <summary>
      /// Will be called before any message handlers are executed.
      /// </summary>
      void OnStart();

      /// <summary>
      /// Will be called after message handlers have executed. Will be called
      /// even in case an error has occurred.
      /// </summary>
      void OnFinished();

      /// <summary>
      /// Will be called in case one of the message handlers throws an exception.
      /// If an error occurrs this method will be called before <see cref="OnFinished"/>.
      /// </summary>
      /// <param name="ex">The exception thrown.</param>
      void OnError(Exception ex);
   }
}
