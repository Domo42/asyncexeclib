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

namespace ExecutionLibTests
{
   using System;
   using Rhino.Mocks.Constraints;

   /// <summary>
   /// Constraint check for method parameter of specific type.
   /// </summary>
   /// <typeparam name="TParam">Type of the parameter.</typeparam>
   public class ParamConstraint<TParam> : AbstractConstraint
   {
      private string _message;

      /// <summary>
      /// Initializes a new instance of the <see cref="ParamConstraintConstraint{TParam}"/> class.
      /// </summary>
      public ParamConstraint()
      {
         _message = typeof(TParam).Name;
      }

      /// <summary>
      /// Gets the message for this constraint
      /// </summary>
      public override string Message
      {
         get { return _message; }
      }

      /// <summary>
      /// Gets the object used as method parameter.
      /// </summary>
      public TParam Param
      {
         get;
         private set;
      }

      /// <summary>
      /// Determines if the object pass the constraints
      /// </summary>
      public override bool Eval(object obj)
      {
         bool pass = false;

         if (obj != null)
         {
            if (obj is TParam)
            {
               pass = true;
            }
            else
            {
               _message = string.Format(
                  "Expected object of type {0} but was {1}.", 
                  typeof(TParam).Name,
                  obj.GetType().Name);
            }
         }
         else
         {
            _message = string.Format("Expected object of type {0} but was <null>.", typeof(TParam).Name);
         }

         return pass;
      }     
   }
}
