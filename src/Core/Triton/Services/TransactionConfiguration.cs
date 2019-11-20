using System;
using System.Collections.Generic;
using TheXDS.Triton.Models.Base;
using TheXDS.Triton.Services.Base;

namespace TheXDS.Triton.Services
{
    public class TransactionConfiguration : ITransactionConfiguration
    {
        private int _connectionTimeout = 15000;
        private readonly List<Func<CrudAction, Model?, ServiceResult?>> _preambles = new List<Func<CrudAction, Model?, ServiceResult?>>();
        private ICrudNotificationSource? _notifier;

        int ITransactionConfiguration.ConnectionTimeout => _connectionTimeout;
        ServiceResult? ITransactionConfiguration.Preamble(CrudAction action, Model? entity)
        {
            foreach (var j in _preambles)
            {
                if (j.Invoke(action, entity) is { } r) return r;
            }
            return null;
        }
        ICrudNotificationSource? ITransactionConfiguration.Notifier => _notifier;

        public TransactionConfiguration SetTimeout(int miliseconds)
        {
            _connectionTimeout = miliseconds;
            return this;
        }

        public TransactionConfiguration AddPreamble(Func<CrudAction, Model?, ServiceResult?> function)
        {
            _preambles.Add(function);
            return this;
        }

        public TransactionConfiguration InsertPreamble(int index, Func<CrudAction, Model?, ServiceResult?> function)
        {
            _preambles.Insert(index, function);
            return this;
        }

        public TransactionConfiguration NotifyVia(ICrudNotificationSource notifier)
        {
            _notifier = notifier;
            return this;
        }
    }
}