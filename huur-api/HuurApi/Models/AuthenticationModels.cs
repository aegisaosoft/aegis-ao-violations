/*
 *
 * Copyright (c) 2024 Alexander Orlov.
 * 34 Middletown Ave Atlantic Highlands NJ 07716
 *
 * THIS SOFTWARE IS THE CONFIDENTIAL AND PROPRIETARY INFORMATION OF
 * Alexander Orlov. ("CONFIDENTIAL INFORMATION"). YOU SHALL NOT DISCLOSE
 * SUCH CONFIDENTIAL INFORMATION AND SHALL USE IT ONLY IN ACCORDANCE
 * WITH THE TERMS OF THE LICENSE AGREEMENT YOU ENTERED INTO WITH
 * Alexander Orlov.
 *
 * Author: Alexander Orlov
 *
 */

using Newtonsoft.Json;

namespace HuurApi.Models;

/// <summary>
/// Request model for user login
/// </summary>
public class LoginRequest
{
    /// <summary>
    /// User's email address
    /// </summary>
    [JsonProperty("email")]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// User's password
    /// </summary>
    [JsonProperty("password")]
    public string Password { get; set; } = string.Empty;
}

/// <summary>
/// Response model for user login
/// </summary>
public class LoginResponse
{
    /// <summary>
    /// Authentication token
    /// </summary>
    [JsonProperty("token")]
    public string Token { get; set; } = string.Empty;

    /// <summary>
    /// Refresh token for obtaining new access tokens
    /// </summary>
    [JsonProperty("refreshToken")]
    public string? RefreshToken { get; set; }

    /// <summary>
    /// User information
    /// </summary>
    [JsonProperty("user")]
    public User User { get; set; } = new();

    /// <summary>
    /// Token expiration date and time
    /// </summary>
    [JsonProperty("expiresAt")]
    public DateTime ExpiresAt { get; set; }
}

/// <summary>
/// Request model for user registration
/// </summary>
public class RegisterRequest
{
    /// <summary>
    /// Username for the account
    /// </summary>
    [JsonProperty("username")]
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// User's email address
    /// </summary>
    [JsonProperty("email")]
    public string Email { get; set; } = string.Empty;

    /// <summary>
    /// User's password
    /// </summary>
    [JsonProperty("password")]
    public string Password { get; set; } = string.Empty;

    /// <summary>
    /// Password confirmation
    /// </summary>
    [JsonProperty("confirmPassword")]
    public string ConfirmPassword { get; set; } = string.Empty;

    /// <summary>
    /// User's first name
    /// </summary>
    [JsonProperty("firstName")]
    public string FirstName { get; set; } = string.Empty;

    /// <summary>
    /// User's last name
    /// </summary>
    [JsonProperty("lastName")]
    public string LastName { get; set; } = string.Empty;

    /// <summary>
    /// User's phone number
    /// </summary>
    [JsonProperty("phoneNumber")]
    public string? PhoneNumber { get; set; }
}

/// <summary>
/// Response model for user registration
/// </summary>
public class RegisterResponse
{
    /// <summary>
    /// Created user information
    /// </summary>
    [JsonProperty("user")]
    public User User { get; set; } = new();

    /// <summary>
    /// Response message
    /// </summary>
    [JsonProperty("message")]
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// Whether email verification is required
    /// </summary>
    [JsonProperty("requiresEmailVerification")]
    public bool RequiresEmailVerification { get; set; } = false;
}

/// <summary>
/// Request model for changing user password
/// </summary>
public class ChangePasswordRequest
{
    /// <summary>
    /// User's current password
    /// </summary>
    [JsonProperty("currentPassword")]
    public string CurrentPassword { get; set; } = string.Empty;

    /// <summary>
    /// New password
    /// </summary>
    [JsonProperty("newPassword")]
    public string NewPassword { get; set; } = string.Empty;

    /// <summary>
    /// New password confirmation
    /// </summary>
    [JsonProperty("confirmNewPassword")]
    public string ConfirmNewPassword { get; set; } = string.Empty;
}

/// <summary>
/// Request model for resetting user password
/// </summary>
public class ResetPasswordRequest
{
    /// <summary>
    /// User's email address
    /// </summary>
    [JsonProperty("email")]
    public string Email { get; set; } = string.Empty;
}

/// <summary>
/// Request model for confirming password reset
/// </summary>
public class ResetPasswordConfirmRequest
{
    /// <summary>
    /// Reset token received via email
    /// </summary>
    [JsonProperty("token")]
    public string Token { get; set; } = string.Empty;

    /// <summary>
    /// New password
    /// </summary>
    [JsonProperty("newPassword")]
    public string NewPassword { get; set; } = string.Empty;

    /// <summary>
    /// New password confirmation
    /// </summary>
    [JsonProperty("confirmNewPassword")]
    public string ConfirmNewPassword { get; set; } = string.Empty;
}

/// <summary>
/// Generic authentication response
/// </summary>
public class AuthResponse
{
    /// <summary>
    /// Whether the operation was successful
    /// </summary>
    [JsonProperty("success")]
    public bool Success { get; set; }

    /// <summary>
    /// Response message
    /// </summary>
    [JsonProperty("message")]
    public string Message { get; set; } = string.Empty;

    /// <summary>
    /// List of error messages if any
    /// </summary>
    [JsonProperty("errors")]
    public List<string>? Errors { get; set; }
}

/// <summary>
/// Request model for refreshing access token
/// </summary>
public class TokenRefreshRequest
{
    /// <summary>
    /// Refresh token
    /// </summary>
    [JsonProperty("refreshToken")]
    public string RefreshToken { get; set; } = string.Empty;
}

/// <summary>
/// Request model for user logout
/// </summary>
public class LogoutRequest
{
    /// <summary>
    /// Authentication token to invalidate
    /// </summary>
    [JsonProperty("token")]
    public string? Token { get; set; }

    /// <summary>
    /// Whether to logout from all devices
    /// </summary>
    [JsonProperty("logoutFromAllDevices")]
    public bool LogoutFromAllDevices { get; set; } = false;
}
