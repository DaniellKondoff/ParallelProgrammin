using System.Threading;

namespace DataSharingAndSync
{
    public class BankAccount
    {
        private int balance;
        //public object padlock = new object();
        public int Balance
        {
            get  {return this.balance; }
            private set { this.balance = value; }
        }

        public void Deposit(int amount)
        {
            //lock (padlock)
            //{
            //    Balance += amount;
            //}
            //
            //Interlocked.Add(ref balance, amount);

            Balance += amount;
        }

        public void Withdraw(int amount)
        {
            //lock (padlock)
            //{
            //    Balance -= amount;
            //}
            //
            //Interlocked.Add(ref balance, -amount);

            Balance -= amount;
        }

        public void Tramsfer(BankAccount where, int amount)
        {
            Balance -= amount;
            where.Balance += amount;
        }
    }
}
