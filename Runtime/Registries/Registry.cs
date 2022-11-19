using System;
using System.Collections.Generic;
using Crysc.Common;

namespace Crysc.Registries
{
    public class Registry<T, TSelf> : SingletonBehaviour<TSelf>
        where T : IRegisterable
        where TSelf : Registry<T, TSelf>
    {
        private readonly HashSet<T> _members = new();

        public event EventHandler Destroying;

        public IEnumerable<T> Members => _members;

        private void OnEnable()
        {
            foreach (T member in Members) SubscribeToEvents(member);
        }

        private void OnDisable()
        {
            foreach (T member in Members) UnsubscribeFromEvents(member);
        }

        public void Register(T member)
        {
            _members.Add(member);
            SubscribeToEvents(member);
        }

        protected virtual void SubscribeToEvents(T member) { member.Destroying += DestroyingEventHandler; }
        protected virtual void UnsubscribeFromEvents(T member) { member.Destroying -= DestroyingEventHandler; }

        private void DestroyingEventHandler(object sender, EventArgs _)
        {
            var member = (T) sender;
            _members.Remove(member);

            Destroying?.Invoke(sender: member, e: EventArgs.Empty);
        }
    }
}
