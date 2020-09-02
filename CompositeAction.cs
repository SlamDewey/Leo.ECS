/*
    Copyright (c) 2020, Jared Massa
    
    Permission is hereby granted, free of charge, to any person obtaining a copy
    of this software and associated documentation files (the "Software"), to deal
    in the Software without restriction, including without limitation the rights
    to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
    copies of the Software, and to permit persons to whom the Software is
    furnished to do so, subject to the following conditions:

    The above copyright notice and this permission notice shall be included in all
    copies or substantial portions of the Software.

    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
    IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
    FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
    AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
    LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
    OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
    SOFTWARE.
*/


using System;
using System.Collections.Generic;

namespace Leo.ECS
{
    /// <summary>
    /// Represents a set of unique delegates, to be treated as a single entity
    /// </summary>
    public class CompositeAction
    {
        /// <summary>
        /// A private list of actions to be performed
        /// </summary>
        private HashSet<Action> actions;

        /// <summary>
        /// A private constructor.  This class only exists if there are actions in it.
        /// </summary>
        private CompositeAction()
        {
            actions = new HashSet<Action>();
        }

        /// <summary>
        /// Clear all the actions from the action list
        /// </summary>
        public void Clear() => actions.Clear();

        /// <summary>
        /// Invoke all actions
        /// </summary>
        public void Invoke()
        {
            foreach (Action a in actions)
                a.Invoke();
        }

        /// <summary>
        /// Add a new action directly to this CompositeAction
        /// </summary>
        /// <param name="a">the delegate to add</param>
        public void Add(Action a) => actions.Add(a);
        /// <summary>
        /// Directly remove an Action from this CompositeAction
        /// </summary>
        /// <param name="a">the delegate to remove</param>
        public void Remove(Action a) => actions.Remove(a);

        /// <summary>
        /// Add two KappaActions together
        /// </summary>
        /// <param name="a">the first KappaAction</param>
        /// <param name="b">the second KappaAction</param>
        /// <returns>a new KappaAction with their lists combined</returns>
        public static CompositeAction operator +(CompositeAction a, CompositeAction b)
        {
            CompositeAction toRet = new CompositeAction();
            if (a != null) toRet.actions.UnionWith(a.actions);
            if (b != null) toRet.actions.UnionWith(b.actions);
            return toRet;
        }

        /// <summary>
        /// Add a delegate reference to this list of action callbacks.
        /// </summary>
        /// <param name="compAct">the composite action to store the reference</param>
        /// <param name="action">the delegate to add</param>
        /// <returns>a new kappa action with a combined list</returns>
        public static CompositeAction operator +(CompositeAction compAct, Action action)
        {
            CompositeAction toRet = new CompositeAction();
            toRet.actions.Add(action);
            if (compAct != null)
                toRet.actions.UnionWith(compAct.actions);
            return toRet;
        }
        /// <summary>
        /// Remove a delegate reference from the list of action callbacks
        /// </summary>
        /// <param name="compAct">the composite action which has the reference</param>
        /// <param name="action">the delegate to remove</param>
        /// <returns>a new kappa action with a combined list</returns>
        public static CompositeAction operator -(CompositeAction compAct, Action action)
        {
            if (compAct == null) return null;
            CompositeAction toRet = new CompositeAction();
            toRet.actions.UnionWith(compAct.actions);
            toRet.actions.Remove(action);
            return toRet;
        }
    }
}
