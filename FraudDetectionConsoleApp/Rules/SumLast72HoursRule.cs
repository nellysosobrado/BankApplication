using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FraudDetectionConsoleApp.Rules
{
    public class SumLast72HoursRule : IFraudRule
    {
        public bool IsSuspicious(Transaction transaction, IEnumerable<Transaction> allTransactions)
        {
            var windowStart = transaction.Date.AddHours(-72);
            var sum = allTransactions
                .Where(t => t.Date >= windowStart && t.Date <= transaction.Date)
                .Sum(t => t.Amount);

            return sum > 23000;
        }
    }
}
