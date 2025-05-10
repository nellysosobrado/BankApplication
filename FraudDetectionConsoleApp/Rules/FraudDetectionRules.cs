using DAL.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FraudDetectionConsoleApp.Rules
{
    public static class FraudDetectionRules
    {
        public static bool IsSuspiciousTransaction(Transaction tx)
            => tx.Amount >= 15000;

        public static bool HasSuspiciousRecentActivity(List<Transaction> allTransactions, DateTime current)
        {
            var windowStart = current.AddHours(-72);
            var sum = allTransactions
                .Where(t => t.Date >= windowStart && t.Date <= current)
                .Sum(t => t.Amount);

            return sum > 23000;
        }
    }
}
