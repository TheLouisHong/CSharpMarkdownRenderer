using System;
using System.Collections.Generic;
using System.Text;

namespace Markdown2HTML.Core.Algorithms
{
    public abstract class DescreteState<TData>
    {
        private BlindDescreteStateMachine<TData> _stateMachine;

        public abstract bool Ask(ref TData data);
        public abstract void Run(ref TData data);

    }

    public class BlindDescreteStateMachine<TData>
    {
        public readonly bool DebugTracking;
        public readonly List<Tuple<DescreteState<TData>, TData>> _stateHistory = new List<Tuple<DescreteState<TData>, TData>>();

        private readonly List<DescreteState<TData>> _statesInOrder;
        private DescreteState<TData> _currentState;

        public DescreteState<TData> CurrentState => _currentState;

        public BlindDescreteStateMachine(
            List<DescreteState<TData>> statesInOrder, 
            bool debugTracking = false)
        {
            DebugTracking = debugTracking;
            _statesInOrder = statesInOrder;

            if (statesInOrder == null)
            {
                throw new ArgumentException("State List cannot be null.");
            }

        }

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

            return sb.ToString();
        }
    }
}