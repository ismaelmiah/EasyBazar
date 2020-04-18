using System;
using System.Collections.Generic;
using System.Text;

namespace DataSets.Utility
{
    public static class ProjectConstant
    {
        public const string ResultNotFound = "Data Not Found";

        //----------------------------------------------------------//
        public const string Role_User_Indi = "Individual Customer";
        public const string Role_Admin = "Admin";
        public const string Role_Employee = "Employee";
        public const string Role_Customer = "Customer";
        //----------------------------------------------------------//

        public const string shoppingCart = "ShoppingCart";

        //PaymentStatus----------------------------------------------------------//

        public const string PaymentStatusPending = "Pending";
        public const string PaymentStatusRejected = "Rejected";
        public const string PaymentStatusApproved = "Approved";
        public const string PaymentStatusDelayedPayment = "Delayed";

        //OrderStatus----------------------------------------------------------//

        public const string StatusPending = "Pending";
        public const string StatusApproved = "Approved";
        public const string StatusInProcess = "Processing";
        public const string StatusShipped = "Shipped";
        public const string StatusCancelled = "Cancelled";
        public const string StatusRefund = "Refund";

        //---------------------------------------------------------------------//

    }
}
