namespace Boilerplate.Authentication.Models.Constants
{
    public static class AccountConstants
    {
        public const string HEADER_KEY = "X-Forwarded-For";
        public const string ORIGIN = "origin";
        public const string REFRESH_TOKEN = "refreshToken";
        public const string ACCOUNT = "Account";

        public const string REGISTER_MESSAGE = "Registration successful, please check your email for verification instructions";
        public const string VERIFY_EMAIL_MESSAGE = "Verification successful, you can now login";
        public const string TOKEN_REQUIRED_MESSAGE = "Token is required";
        public const string UNAUTHORIZED_MESSAGE = "Unauthorized";
        public const string TOKEN_REVOKED_MESSAGE = "Token revoked";
        public const string FORGOT_PASSWORD_MESSAGE = "Please check your email for password reset instructions";
        public const string VALIDATE_TOKEN_MESSAGE = "Token is valid";
        public const string RESET_PASSWORD_MESSAGE = "Password reset successful, you can now login";
        public const string DELETE_MESSAGE = "Account deleted successfully";

        public const string EMAIL_EXCPETION = "Email is incorrect";
        public const string PASSWORD_EXCPETION = "Password is incorrect";
        public const string VERIFY_EMAIL_EXCPETION = "Verification failed";
        public const string TOKEN_EXCPETION = "Invalid token";
        public const string EMAIL_REGISTERED_EXCPETION = "Email {0} is already registered";
        public const string ACCOUNT_EXCPETION = "Account not found";
    }
}
