import React from 'react';
import { BrowserRouter as Router, Routes, Route, Navigate } from 'react-router-dom';
import Login from './pages/Login/Login';
import Register from './pages/Register/Register';
import ProfilePage from './pages/ProfilePage';
import Navbar from './components/Navbar/Navbar';

function App() {
  return (
    <Router>
      <div className="app">
        <Navbar />
        <Routes>
          <Route path="/" element={<Navigate to="/login" replace />} />
          <Route path="/login" element={<Login />} />
          <Route path="/register" element={<Register />} />
          <Route path="/profile" element={<ProfilePage />} />
        </Routes>
      </div>
    </Router>
  );
}

export default App;