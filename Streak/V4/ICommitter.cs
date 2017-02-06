using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Streak.V4
{
    public interface ICommitter
    {
        void Register(Action commit);
        void Update();
    }

    public class Committer : ICommitter
    {
        //private readonly List<ICommittable> _committables;

        private readonly List<Action> _committables;
        private readonly int _rate;
        private readonly int _time;

        private int _updates;

        public Committer(int rate, int time)
        {
            _committables = new List<Action>();
            _rate = rate;
            _time = time;
        }

        public void Register(Action commit)
        {
            _committables.Add(commit);
        }

        public void Update()
        {
            _updates++;

            if (_updates < _rate) return;

            foreach (var commit in _committables)
            {
                commit();
            }

            _updates = 0;
        }
    }
}