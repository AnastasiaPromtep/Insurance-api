namespace InsuranceApi.Errors;

public static class ErrorCodes
{
    public static class Policy
    {
        public const string NotFound = "policy_not_found";
        public const string Duplicate = "policy_number_already_exists";
        public const string InvalidPolicyNumber = "invalid_policy_number";
        public const string InvalidSubscriberName = "invalid_subscriber_name";
        public const string InvalidPremiumAmount = "invalid_premium_amount";
    }
}