import React from 'react';
import { Link, useNavigate } from 'react-router-dom';
import './Navbar.css';

const Navbar = () => {
  const isLoggedIn = false; // This will be replaced with actual auth state
  const navigate = useNavigate();

  const handleLogout = () => {
    // TODO: Add logout logic here
    console.log('Logging out...');
    navigate('/login');
  };

  return (
    <nav className="navbar">
      <div className="navbar-brand">
        <Link to="/">
          <h1>Habyx</h1>
        </Link>
      </div>
      <div className="navbar-menu">
        {isLoggedIn ? (
          <>
            <Link to="/profile">Profile</Link>
            <Link to="/friends">Friends</Link>
            <Link to="/chat">Chat</Link> {/* Added Chat link */}
            <button className="logout-btn" onClick={handleLogout}>
              Logout
            </button>
          </>
        ) : (
          <>
            <Link to="/login">Login</Link>
            <Link to="/register">Register</Link>
          </>
        )}
      </div>
    </nav>
  );
};

export default Navbar;