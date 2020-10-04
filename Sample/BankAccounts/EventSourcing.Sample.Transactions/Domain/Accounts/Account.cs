using System;
using Core.Aggregates;
using EventSourcing.Sample.Tasks.Contracts.Accounts.Events;
using EventSourcing.Sample.Tasks.Contracts.Transactions;
using EventSourcing.Sample.Tasks.Contracts.Transactions.Events;
using EventSourcing.Sample.Transactions.Domain.Accounts;

namespace EventSourcing.Sample.Tasks.Domain.Accounts
{
    public class Account: Aggregate
    {
        public Guid ClientId { get; private set; }

        public decimal Balance { get; private set; }

        public string Number { get; private set; }

        public Account()
        {
        }

        public Account(Guid clientId, IAccountNumberGenerator accountNumberGenerator)
        {
            var @event = new NewAccountCreated
            {
                AccountId = Guid.NewGuid(),
                ClientId = clientId,
                Number = accountNumberGenerator.Generate()
            };

            Apply(@event);
            Enqueue(@event);
        }

        public void RecordInflow(Guid fromId, decimal ammount)
        {
            var @event = new NewInflowRecorded(fromId, Id, new Inflow(ammount, DateTime.Now));

            Apply(@event);
            Enqueue(@event);
        }

        public void RecordOutflow(Guid toId, decimal ammount)
        {
            var @event = new NewOutflowRecorded(Id, toId, new Outflow(ammount, DateTime.Now));

            Apply(@event);
            Enqueue(@event);
        }

        public void Apply(NewAccountCreated @event)
        {
            Id = @event.AccountId;
            ClientId = @event.ClientId;
            Number = @event.Number;
        }

        public void Apply(NewInflowRecorded @event)
        {
            Balance += @event.Inflow.Ammount;
        }

        public void Apply(NewOutflowRecorded @event)
        {
            Balance -= @event.Outflow.Ammount;
        }
    }
}
