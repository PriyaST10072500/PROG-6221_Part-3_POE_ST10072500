using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PROG_6221_Part_3_POE_ST10072500
{
    public delegate void CaloriesExceededEventHandler(object sender, EventArgs e);

    public class CaloriesAmount
    {
        private double calorieLimit;
        private double calorieTotalAmt;

        public event CaloriesExceededEventHandler CaloriesExceeded;

        private CaloriesAmount() { }

        public CaloriesAmount(double calorieLimit, double calorieTotalAmt)
        {
            this.calorieLimit = calorieLimit;
            this.calorieTotalAmt = calorieTotalAmt;
        }

        public double CalorieLimit => calorieLimit;
        public double CalorieTotalAmount => calorieTotalAmt;

        public void Credit(double amount)
        {
            if (amount < 0)
                throw new ArgumentOutOfRangeException(nameof(amount));

            calorieTotalAmt += amount;
            CheckCalories();
        }

        protected virtual void OnCaloriesExceeded(EventArgs e)
        {
            CaloriesExceeded?.Invoke(this, e);
        }

        private void CheckCalories()
        {
            if (calorieTotalAmt > 300)
                OnCaloriesExceeded(EventArgs.Empty);
        }
    }
}

