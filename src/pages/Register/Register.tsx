import React, { useState } from 'react';
import { useNavigate } from 'react-router-dom';
import { api, type RegisterData } from '../../services/api';
import { validatePassword, validateEmail } from '../../utils/validation';
import './Register.css';

const Register = () => {
  const navigate = useNavigate();
  const [formData, setFormData] = useState<RegisterData>({
    firstName: '',
    lastName: '',
    email: '',
    password: ''
  });
  const [confirmPassword, setConfirmPassword] = useState('');
  const [errors, setErrors] = useState({
    firstName: '',
    lastName: '',
    email: '',
    password: '',
    confirmPassword: ''
  });
  const [loading, setLoading] = useState(false);
  const [showPassword, setShowPassword] = useState(false);
  const [showConfirmPassword, setShowConfirmPassword] = useState(false);

  const getPasswordStrength = (password: string): { strength: number; label: string } => {
    if (password.length === 0) return { strength: 0, label: '' };
    
    let strength = 0;
    if (password.length >= 8) strength += 1;
    if (/[A-Z]/.test(password)) strength += 1;
    if (/[a-z]/.test(password)) strength += 1;
    if (/[0-9]/.test(password)) strength += 1;
    if (/[!@#$%^&*]/.test(password)) strength += 1;

    const labels = ['Weak', 'Fair', 'Good', 'Strong', 'Excellent'];
    return { 
      strength, 
      label: strength > 0 ? labels[strength - 1] : ''
    };
  };

  const handleChange = (e: React.ChangeEvent<HTMLInputElement>) => {
    const { name, value } = e.target;
    
    setFormData(prev => ({
      ...prev,
      [name]: value
    }));

    // Clear specific error when user types
    setErrors(prev => ({
      ...prev,
      [name]: ''
    }));

    // Validate password
    if (name === 'password') {
      const passwordErrors = validatePassword(value);
      if (passwordErrors.length > 0) {
        setErrors(prev => ({
          ...prev,
          password: passwordErrors[0]
        }));
      }
      // Check password match
      if (confirmPassword && value !== confirmPassword) {
        setErrors(prev => ({
          ...prev,
          confirmPassword: 'Passwords do not match'
        }));
      }
    }

    // Validate email
    if (name === 'email') {
      const emailErrors = validateEmail(value);
      if (emailErrors.length > 0) {
        setErrors(prev => ({
          ...prev,
          email: emailErrors[0]
        }));
      }
    }
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();

    if (formData.password !== confirmPassword) {
      setErrors(prev => ({
        ...prev,
        confirmPassword: 'Passwords do not match'
      }));
      return;
    }

    setErrors({
      firstName: '',
      lastName: '',
      email: '',
      password: '',
      confirmPassword: ''
    });
    setLoading(true);

    try {
      console.log('Submitting registration data:', formData); // Debug log
      const response = await api.register(formData);
      console.log('Registration response:', response); // Debug log
      
      if (response.id) {
        navigate('/login');
      } else {
        setErrors(prev => ({
          ...prev,
          email: 'Registration failed'
        }));
      }
    } catch (err) {
      console.error('Registration error details:', err); // Detailed error log
      setErrors(prev => ({
        ...prev,
        email: 'An error occurred during registration'
      }));
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="register-container">
      <form onSubmit={handleSubmit} className="register-form">
        <h2>Create Account</h2>

        <div className="form-group">
          <input
            type="text"
            name="firstName"
            placeholder="First Name"
            value={formData.firstName}
            onChange={handleChange}
            required
            className="form-input"
          />
          {errors.firstName && (
            <div className="error-message">{errors.firstName}</div>
          )}
        </div>

        <div className="form-group">
          <input
            type="text"
            name="lastName"
            placeholder="Last Name"
            value={formData.lastName}
            onChange={handleChange}
            required
            className="form-input"
          />
          {errors.lastName && (
            <div className="error-message">{errors.lastName}</div>
          )}
        </div>

        <div className="form-group">
          <input
            type="email"
            name="email"
            placeholder="Email"
            value={formData.email}
            onChange={handleChange}
            required
            className="form-input"
          />
          {errors.email && (
            <div className="error-message">{errors.email}</div>
          )}
        </div>

        <div className="form-group password-group">
          <input
            type={showPassword ? "text" : "password"}
            name="password"
            placeholder="Password"
            value={formData.password}
            onChange={handleChange}
            required
            className="form-input"
          />
          <button
            type="button"
            className="toggle-password"
            onClick={() => setShowPassword(!showPassword)}
          >
            {showPassword ? "Hide" : "Show"}
          </button>
        </div>

        {formData.password && (
          <div className="password-strength-container">
            <div className="password-strength-label">
              Password Strength: 
              <span className={`strength-${getPasswordStrength(formData.password).label.toLowerCase()}`}>
                {getPasswordStrength(formData.password).label}
              </span>
            </div>
            <div className="strength-bar">
              {[...Array(5)].map((_, index) => (
                <div 
                  key={index}
                  className={`strength-segment ${
                    index < getPasswordStrength(formData.password).strength ? 'filled' : ''
                  }`}
                />
              ))}
            </div>
          </div>
        )}

        {errors.password && (
          <div className="error-message">{errors.password}</div>
        )}

        <div className="form-group password-group">
          <input
            type={showConfirmPassword ? "text" : "password"}
            placeholder="Confirm Password"
            value={confirmPassword}
            onChange={(e) => {
              setConfirmPassword(e.target.value);
              if (e.target.value !== formData.password) {
                setErrors(prev => ({
                  ...prev,
                  confirmPassword: 'Passwords do not match'
                }));
              } else {
                setErrors(prev => ({
                  ...prev,
                  confirmPassword: ''
                }));
              }
            }}
            required
            className="form-input"
          />
          <button
            type="button"
            className="toggle-password"
            onClick={() => setShowConfirmPassword(!showConfirmPassword)}
          >
            {showConfirmPassword ? "Hide" : "Show"}
          </button>
        </div>

        {errors.confirmPassword && (
          <div className="error-message">{errors.confirmPassword}</div>
        )}

        <button 
          type="submit" 
          disabled={loading || Object.values(errors).some(error => error !== '')}
          className="submit-button"
        >
          {loading ? 'Creating Account...' : 'Register'}
        </button>

        <div className="form-footer">
          <p>
            Already have an account? {' '}
            <span 
              onClick={() => navigate('/login')}
              className="login-link"
            >
              Login here
            </span>
          </p>
        </div>
      </form>
    </div>
  );
};

export default Register;