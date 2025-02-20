import React, { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { api, type LoginCredentials } from '../../services/api';
import { validatePassword, validateEmail } from '../../utils/validation';
import './Login.css';

const Login: React.FC = () => {
  const navigate = useNavigate();
  const [credentials, setCredentials] = useState<LoginCredentials>({
    email: '',
    password: ''
  });
  const [passwordErrors, setPasswordErrors] = useState<string[]>([]);
  const [emailErrors, setEmailErrors] = useState<string[]>([]);
  const [loginError, setLoginError] = useState('');
  const [loading, setLoading] = useState(false);
  const [showPassword, setShowPassword] = useState(false);
  const [rememberMe, setRememberMe] = useState(false);

  useEffect(() => {
    const savedEmail = localStorage.getItem('rememberedEmail');
    if (savedEmail) {
      setCredentials(prev => ({ ...prev, email: savedEmail }));
      setRememberMe(true);
    }
  }, []);

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
    const { name, value, type, checked } = e.target;
    
    if (type === 'checkbox') {
      setRememberMe(checked);
    } else {
      setCredentials(prev => ({
        ...prev,
        [name]: value
      }));

      if (name === 'password') {
        setPasswordErrors(validatePassword(value));
      }
      if (name === 'email') {
        setEmailErrors(validateEmail(value));
      }
    }
    setLoginError('');
  };

  const handleSubmit = async (e: React.FormEvent) => {
    e.preventDefault();
    
    const currentEmailErrors = validateEmail(credentials.email);
    const currentPasswordErrors = validatePassword(credentials.password);

    if (currentEmailErrors.length > 0 || currentPasswordErrors.length > 0) {
      setEmailErrors(currentEmailErrors);
      setPasswordErrors(currentPasswordErrors);
      return;
    }

    setLoading(true);
    try {
      const response = await api.login(credentials);
      
      if (rememberMe) {
        localStorage.setItem('rememberedEmail', credentials.email);
      } else {
        localStorage.removeItem('rememberedEmail');
      }

      localStorage.setItem('token', response.token);
      if (response.user) {
        localStorage.setItem('user', JSON.stringify(response.user));
      }
      
      navigate('/profile');
    } catch (err) {
      setLoginError(err instanceof Error ? err.message : 'Login failed. Please try again.');
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="login-container">
      <form onSubmit={handleSubmit} className="login-form">
        <h2>Welcome to Habyx</h2>
        
        {loginError && (
          <div className="error-message">
            {loginError}
          </div>
        )}

        <div className="form-group">
          <input
            type="email"
            name="email"
            placeholder="Email"
            value={credentials.email}
            onChange={handleChange}
            required
            className="form-input"
          />
          {emailErrors.length > 0 && credentials.email.length > 0 && (
            <div className="error-container">
              {emailErrors.map((error, index) => (
                <div key={index} className="error-message">
                  {error}
                </div>
              ))}
            </div>
          )}
        </div>

        <div className="form-group password-group">
          <input
            type={showPassword ? "text" : "password"}
            name="password"
            placeholder="Password"
            value={credentials.password}
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

        {credentials.password && (
          <div className="password-strength-container">
            <div className="password-strength-label">
              Password Strength: 
              <span className={`strength-${getPasswordStrength(credentials.password).label.toLowerCase()}`}>
                {getPasswordStrength(credentials.password).label}
              </span>
            </div>
            <div className="strength-bar">
              {[...Array(5)].map((_, index) => (
                <div 
                  key={index}
                  className={`strength-segment ${
                    index < getPasswordStrength(credentials.password).strength ? 'filled' : ''
                  }`}
                />
              ))}
            </div>
          </div>
        )}

        {passwordErrors.length > 0 && credentials.password.length > 0 && (
          <div className="error-container">
            {passwordErrors.map((error, index) => (
              <div key={index} className="error-message">
                {error}
              </div>
            ))}
          </div>
        )}

        <div className="form-options">
          <div className="remember-me">
            <label>
              <input
                type="checkbox"
                name="rememberMe"
                checked={rememberMe}
                onChange={handleChange}
                className="remember-checkbox"
              />
              <span>Remember me</span>
            </label>
          </div>
          <div className="forgot-password">
            <span onClick={() => navigate('/forgot-password')}>
              Forgot Password?
            </span>
          </div>
        </div>

        <button 
          type="submit" 
          disabled={loading || emailErrors.length > 0 || passwordErrors.length > 0}
          className="submit-button"
        >
          {loading ? 'Logging in...' : 'Login'}
        </button>

        <div className="form-footer">
          <p>
            Don't have an account? {' '}
            <span 
              onClick={() => navigate('/register')}
              className="register-link"
            >
              Register here
            </span>
          </p>
        </div>
      </form>
    </div>
  );
};

export default Login;