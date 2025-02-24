// src/utils/validation.ts

export const validatePassword = (password: string): string[] => {
    const errors: string[] = [];
  
    if (password.length < 8) {
      errors.push('Password must be at least 8 characters long');
    }
    if (!/[A-Z]/.test(password)) {
      errors.push('Must contain an uppercase letter');
    }
    if (!/[a-z]/.test(password)) {
      errors.push('Must contain a lowercase letter');
    }
    if (!/[0-9]/.test(password)) {
      errors.push('Must contain a number');
    }
    if (!/[!@#$%^&*]/.test(password)) {
      errors.push('Must contain a special character (!@#$%^&*)');
    }
  
    return errors;
  };
  
  export const validateEmail = (email: string): string[] => {
    const errors: string[] = [];
    
    // Basic email format validation
    const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    if (!emailRegex.test(email)) {
      errors.push('Please enter a valid email address');
    }
  
    // Common domain validation
    const domain = email.split('@')[1];
    if (domain) {
      if (!domain.includes('.')) {
        errors.push('Invalid email domain');
      }
      if (domain.endsWith('.')) {
        errors.push('Email domain cannot end with a dot');
      }
    }
  
    // Length validation
    if (email.length > 254) {
      errors.push('Email is too long');
    }
  
    return errors;
  };