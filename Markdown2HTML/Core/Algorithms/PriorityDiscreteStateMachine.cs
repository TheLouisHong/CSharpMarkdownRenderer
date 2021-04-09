using System;
using System.Collections.Generic;
using System.Text;

namespace Markdown2HTML.Core.Algorithms
{
    /// <summary>
    /// A state machine state. used by <see cref="PriorityDiscreteStateMachine{TData}"/>.
    /// </summary>
    /// <typeparam name="TData"></typeparam>
    public abstract class DescreteState<TData>
    {
        private PriorityDiscreteStateMachine<TData> _stateMachine;

        /// <summary>
        /// State machine is asked to be the next state.
        /// </summary>
        /// <param name="data">Current StateMachine data-state.</param>
        /// <returns>
        /// Return true if this state needs to be the next state.
        /// Return false if this state cannot be the next state.
        /// </returns>
        public abstract bool Ask(ref TData data);
        /// <summary>
        /// The state is the current state and is asked to be ran, once.
        /// Since this is not a real time application, all operations are expected to be ran in one function call.
        /// </summary>
        /// <param name="data">Current StateMachine data-state.</param>
        public abstract void Run(ref TData data);

    }

    /// <summary>
    /// The Priority-Discrete-State-Machine.
    /// 
    /// • Discrete because there is no transitioning half-states.
    /// • Priority, something I invented, is that each state is asked in-order of priority to be the next state.
    ///
    /// Each state is asked in-order to be the next state through the <see cref="DescreteState{TData}.Ask"/> function.
    /// If the return value is true, the state becomes the next state and <see cref="DescreteState{TData}.Run"/> is ran.
    ///
    /// The state machine keeps track of <see cref="TData"/> for all states, and is the data all states operate on.
    /// The output of the state machine is said data.
    /// </summary>
    /// <typeparam name="TData"></typeparam>
    public class PriorityDiscreteStateMachine<TData>
    {
        //public readonly bool DebugTracking;
        //public readonly List<Tuple<DescreteState<TData>, TData>> _stateHistory = new List<Tuple<DescreteState<TData>, TData>>();

        /// <summary>
        /// The states, in-order, defined by the user.
        /// </summary>
        private readonly List<DescreteState<TData>> _statesInOrder;
        /// <summary>
        /// Current running state.
        /// </summary>
        private DescreteState<TData> _currentState;

        /// <summary>
        /// Current running state.
        /// </summary>
        public DescreteState<TData> CurrentState => _currentState;
    
        /// <summary>
        /// Construct the state machine.
        /// </summary>
        /// <param name="statesInOrder">The states, in order that they are asked to be the next state.</param>
        public PriorityDiscreteStateMachine(
            List<DescreteState<TData>> statesInOrder
            /*bool debugTracking = false*/)
        {
            //DebugTracking = debugTracking;
            _statesInOrder = statesInOrder;

            if (statesInOrder == null)
            {
                throw new ArgumentException("State List cannot be null.");
            }

        }

        /// <summary>
        /// User runs the state machine.
        /// When no more states can be the next state, the state machine finishes ane exits.
        /// </summary>
        /// <param name="data"></param>
        public void Run(ref TData data)
        {
            while (true)
            {
                var finished = true;
                foreach (var state in _statesInOrder)
                {
                    if (state.Ask(ref data))
                    {
                        state.Run(ref data);
                        finished = false;
                        break;
                    }
                }

                if (finished)
                {
                    break;
                }
            }
        }


        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.Append("State Machine: {");
            for (var i = 0; i < _statesInOrder.Count - 1; i++)
            {
                var state = _statesInOrder[i];
                sb.Append($"{state.GetType().Name}, ");
            }
            sb.AppendLine($"{_statesInOrder[_statesInOrder.Count - 1].GetType().Name}}}");

            /*
            if (DebugTracking)
            {
                sb.AppendLine("History: ");
                for (var i = 0; i < _stateHistory.Count - 1; i++)
                {
                    var row = _stateHistory[i];
                    sb.AppendLine($"{row.Item1.GetType().Name} : {row.Item2}");
                }

                var lastRow = _stateHistory[_stateHistory.Count - 1];
                    sb.AppendLine($"{lastRow.Item1.GetType().Name} : {lastRow.Item2}");
            }
            */

            return sb.ToString();
        }
    }
}