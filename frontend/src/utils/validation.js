export function validatePassword(password) {
  const requirements = [
    [password.length >= 8, "at least 8 characters"],
    [/[A-Z]/.test(password), "one uppercase letter"],
    [/[a-z]/.test(password), "one lowercase letter"],
    [/[0-9]/.test(password), "one number"],
    [/[^A-Za-z0-9]/.test(password), "one special character"],
  ];
  const missing = requirements.filter(([valid]) => !valid).map(([, label]) => label);
  return missing.length ? `Password must contain ${missing.join(", ")}.` : "";
}

export function validateRequiredText(value, label) {
  return value.trim() ? "" : `${label} is required.`;
}
