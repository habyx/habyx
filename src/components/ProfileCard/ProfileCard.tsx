import React, { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { api, type Profile } from '../../services/api';
import './ProfileCard.css';

const ProfileCard: React.FC = () => {
  const navigate = useNavigate();
  const [profile, setProfile] = useState<Profile | null>(null);
  const [isEditing, setIsEditing] = useState(false);
  const [loading, setLoading] = useState(true);
  const [error, setError] = useState('');

  useEffect(() => {
    const token = localStorage.getItem('token');
    if (!token) {
      navigate('/login');
      return;
    }
    fetchProfile();
  }, [navigate]);

  const fetchProfile = async () => {
    try {
      const token = localStorage.getItem('token');
      if (!token) {
        setError('Please log in to view your profile');
        navigate('/login');
        return;
      }
      
      const data = await api.getProfile(token);
      if (!data) {
        throw new Error('No profile data received');
      }
      setProfile(data);
      setError('');
    } catch (err) {
      console.error('Profile loading error:', err);
      setError('Failed to load profile. Please try logging in again.');
      localStorage.removeItem('token'); // Clear invalid token
      navigate('/login');
    } finally {
      setLoading(false);
    }
  };

  const handleEditClick = () => {
    setIsEditing(true);
  };

  const handleSaveClick = async () => {
    try {
      const token = localStorage.getItem('token');
      if (!token || !profile) return;

      await api.updateProfile(token, profile);
      setIsEditing(false);
      fetchProfile();
    } catch (err) {
      setError('Failed to update profile');
    }
  };

  if (loading) return <div>Loading...</div>;
  if (error) return <div className="error-message">{error}</div>;
  if (!profile) return <div>No profile found</div>;

  return (
    <div className="profile-card">
      <div className="profile-header">
        <div className="profile-image">
          <img 
            src={profile.profileImageUrl || '/default-avatar.png'} 
            alt={`${profile.firstName} ${profile.lastName}`} 
          />
        </div>
        <div className="profile-info">
          {isEditing ? (
            <div className="edit-form">
              <input
                type="text"
                value={profile.firstName}
                onChange={(e) => setProfile({...profile, firstName: e.target.value})}
                placeholder="First Name"
              />
              <input
                type="text"
                value={profile.lastName}
                onChange={(e) => setProfile({...profile, lastName: e.target.value})}
                placeholder="Last Name"
              />
              <input
                type="text"
                value={profile.occupation || ''}
                onChange={(e) => setProfile({...profile, occupation: e.target.value})}
                placeholder="Occupation"
              />
              <textarea
                value={profile.bio || ''}
                onChange={(e) => setProfile({...profile, bio: e.target.value})}
                placeholder="Bio"
              />
              <input
                type="text"
                value={profile.location || ''}
                onChange={(e) => setProfile({...profile, location: e.target.value})}
                placeholder="Location"
              />
              <input
                type="text"
                value={profile.skills || ''}
                onChange={(e) => setProfile({...profile, skills: e.target.value})}
                placeholder="Skills"
              />
              <input
                type="text"
                value={profile.education || ''}
                onChange={(e) => setProfile({...profile, education: e.target.value})}
                placeholder="Education"
              />
            </div>
          ) : (
            <>
              <h2>{`${profile.firstName} ${profile.lastName}`}</h2>
              <h3>{profile.occupation || 'No occupation set'}</h3>
              <p className="location">{profile.location || 'No location set'}</p>
              <p className="bio">{profile.bio || 'No bio available'}</p>
              <div className="profile-details">
                <h4>Skills</h4>
                <p>{profile.skills || 'No skills listed'}</p>
                <h4>Education</h4>
                <p>{profile.education || 'No education listed'}</p>
              </div>
            </>
          )}
        </div>
      </div>
      <div className="profile-actions">
        {isEditing ? (
          <button onClick={handleSaveClick} className="save-btn">Save Changes</button>
        ) : (
          <button onClick={handleEditClick} className="edit-btn">Edit Profile</button>
        )}
      </div>
    </div>
  );
};

export default ProfileCard;