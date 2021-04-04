using System;
using System.Collections.Generic;
using System.Text;

namespace Markdown2HTML.Core.Algorithms
{
    public abstract class DescreteState<TTransitionData>
    {
        private BlindDescreteStateMachine<TTransitionData> _stateMachine;

        public abstract bool AskForTransition(TTransitionData transitionData);

        public abstract void OnEnter(TTransitionData transitionData);
        public abstract void Update();
        public abstract void OnExit();

    }

    public class BlindDescreteStateMachine<TTransitionData>
    {
        public readonly bool DebugTracking;
        public readonly List<Tuple<DescreteState<TTransitionData>, TTransitionData>> _stateHistory = new List<Tuple<DescreteState<TTransitionData>, TTransitionData>>();

        private readonly List<DescreteState<TTransitionData>> _statesInOrder;
        private DescreteState<TTransitionData> _currentState;

        public DescreteState<TTransitionData> CurrentState => _currentState;

        public BlindDescreteStateMachine(
            List<DescreteState<TTransitionData>> statesInOrder, 
            bool debugTracking = false)
        {
            DebugTracking = debugTracking;
            _statesInOrder = statesInOrder;

            if (statesInOrder == null)
            {
                throw new ArgumentException("State List cannot be null.");
            }

        }

        public bool Run(TTransitionData bootTransitionData)
        {
            return TryTransition(bootTransitionData);
        }

        public bool TryTransition(TTransitionData transitionData)
        {
            // try each state, in order
            foreach (var potentialState in _statesInOrder)
            {
                // ... can this state be transitioned?
                if (potentialState.AskForTransition(transitionData))
                {
                    // yes.

                    // debug tracking
                    if (DebugTracking)
                    {
                        _stateHistory.Add(new Tuple<DescreteState<TTransitionData>, TTransitionData>(potentialState, transitionData));
                    }

                    // exit previous state.
                    _currentState.OnExit();
                    // new state.
                    _currentState = potentialState;
                    // enter new state.
                    potentialState.OnEnter(transitionData);
                    return true;
                }
            }
            return false;
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